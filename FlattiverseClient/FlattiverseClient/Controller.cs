using System;
using System.Collections.Generic;
using System.Threading;
using Flattiverse;

namespace FlattiverseClient
{
    public class Controller
    {
        private const string EMail      = "kowiit00@hs-esslingen.de";
        private const string Password   = "ZBU0RXSX94M92KRP";
        private const string Nickname   = "Saladino";
        private const string ShipType   = "Scout1";
        private const string ShipName   = "FliegendeTasse";

        private Connector _connector;
        private bool _running;
        private UniverseGroup _universeGroup;
        private Ship _ship;
        private List<FlattiverseMessage> _messages = new List<FlattiverseMessage>();
        ReaderWriterLock messageLock = new ReaderWriterLock();
        private float _scanAngle;
        private Map _map = new Map();
        private int _yImpulse;
        private int _xImpulse;

        public delegate void FlattiveseChanged();

        public event FlattiveseChanged NewMessageEvent;
        public event FlattiveseChanged NewScanEvent;

        public List<FlattiverseMessage> Messages
        {
            get
            {
                messageLock.AcquireReaderLock(100);
                List<FlattiverseMessage> listCopy = new List<FlattiverseMessage>(_messages);
                messageLock.ReleaseReaderLock();
                return listCopy;
            }
        }

        public bool Connected { get; set; }

        public int EnergyPercent
        {
            get { return (int) (_ship.Energy/_ship.EnergyMax*100); }
        }

        public List<Unit> Units
        {
            get { return _map.Units; }
        }

        public bool ShipReady => _ship != null;

        public float ShipRadius
        {
            get
            {
                return ShipReady ? _ship.Radius : -1;
            }
        }

        public void Connect()
        {
            _connector = new Connector(EMail, Password);
            Connected = true;
        }

        public void ListUniverses()
        {
            foreach (var ug in _connector.UniverseGroups)
            {
                Console.WriteLine($"{ug.Name} - {ug.Description} - {ug.Difficulty} - max. {ug.MaxShipsPerPlayer} ships");

                foreach (var universe in ug.Universes)
                {
                    Console.WriteLine($"\tUniverse: {universe.name}");
                }
                foreach (var team in ug.Teams)
                {
                    Console.WriteLine($"\tTeam: {team.Name}");
                }
            }
        }

        public void Disconnect()
        {
            _running = false;
        }

        public void Enter(string universeGroupName, string teamName = "None")
        {
            _universeGroup = _connector.UniverseGroups[universeGroupName];
            Team team = _universeGroup.Teams[teamName];

            _universeGroup.Join(Nickname, team);

            _ship = _universeGroup.RegisterShip(ShipType, ShipName);

            Thread thread = new Thread(Run) {Name = "MainLoop"};
            thread.Start();
        }

        private void Run()
        {
            _running = true;
            UniverseGroupFlowControl flowControl = _universeGroup.GetNewFlowControl();
            _ship.Continue();
            while (_running)
            {
                if(!_ship.IsAlive || !_ship.IsActive) Continue();

                GetPendingMessages();
                flowControl.Commit();
                Scan();
                Move();
                flowControl.Wait();
            }
            _connector.Close();
            Connected = false;
        }

        private void Continue()
        {
            if (!_ship.IsAlive || !_ship.IsActive)
            {
                _map = new Map();
                _ship.Continue();
            }
        }

        private void GetPendingMessages()
        {
            FlattiverseMessage message;
            bool messagesReceived = false;
            messageLock.AcquireWriterLock(100);

            while (_connector.NextMessage(out message))
            {
                _messages.Add(message);
                messagesReceived = true;
            }
            messageLock.ReleaseWriterLock();

            if (messagesReceived)
            {
                NewMessageEvent?.Invoke();
            }
        }

        private void Scan()
        {
            _scanAngle = (_scanAngle + _ship.ScannerDegreePerScan < 360)
                ? _scanAngle + _ship.ScannerDegreePerScan - 2
                : _scanAngle + _ship.ScannerDegreePerScan - 362;
            try
            {
                List<Unit> scannedUnits = _ship.Scan(_scanAngle, _ship.ScannerArea.Limit);
                _map.Insert(scannedUnits);
                NewScanEvent?.Invoke();
            }
            catch (GameException e)
            {
                if (e.ErrorNumber == 84) Continue();
                else
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Impulse(int x, int y)
        {
            _xImpulse += x;
            _yImpulse += y;
        }

        private void Move()
        {
            float direction;
            Console.WriteLine($"{_xImpulse}, {_yImpulse}");
            if (_xImpulse > 0)
            {
                if (_yImpulse > 0) direction = 315;
                else if (_yImpulse < 0) direction = 45;
                else
                {
                    direction = 0;
                }
            }
            else if (_xImpulse < 0)
            {
                if (_yImpulse > 0) direction = 135;
                else if (_yImpulse < 0) direction = 225;
                else
                {
                    direction = 180;
                }
            }
            else
            {
                if (_yImpulse > 0) direction = 90;
                else if (_yImpulse < 0) direction = 270;
                else
                {
                    direction = 0;
                }
            }

            if (_xImpulse != 0 || _yImpulse != 0)
            {
                Vector acceleration = Vector.FromAngleLength(direction, _ship.EngineAcceleration.Limit);
                try
                {
                    _ship.Move(acceleration);

                }
                catch (GameException e)
                {
                    if (e.ErrorNumber == 84) Continue();
                    else
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            _xImpulse = 0;
            _yImpulse = 0;
        }
    }
}