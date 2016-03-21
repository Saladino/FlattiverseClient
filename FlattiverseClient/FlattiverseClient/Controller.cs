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
        private UniverseGroup universeGroup;
        private Ship _ship;
        private List<FlattiverseMessage> _messages = new List<FlattiverseMessage>();
        ReaderWriterLock messageLock = new ReaderWriterLock(); 

        public delegate void FlattiveseChanged();

        public event FlattiveseChanged NewMessageEvent;

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

        public void Connect()
        {
            _connector = new Connector(EMail, Password);
        }

        public void ListUniverses()
        {
            foreach (var universeGroup in _connector.UniverseGroups)
            {
                Console.WriteLine($"{universeGroup.Name} - {universeGroup.Description} - {universeGroup.Difficulty} - max. {universeGroup.MaxShipsPerPlayer} ships");

                foreach (var universe in universeGroup.Universes)
                {
                    Console.WriteLine($"\tUniverse: {universe.name}");
                }
                foreach (var team in universeGroup.Teams)
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
            universeGroup = _connector.UniverseGroups[universeGroupName];
            Team team = universeGroup.Teams[teamName];

            universeGroup.Join(Nickname, team);

            _ship = universeGroup.RegisterShip(ShipType, ShipName);

            Thread thread = new Thread(Run);
            thread.Name = "MainLoop";
            thread.Start();
        }

        private void Run()
        {
            _running = true;
            UniverseGroupFlowControl flowControl = universeGroup.GetNewFlowControl();

            while (_running)
            {
                GetPendingMessages();
                flowControl.Commit();
                flowControl.Wait();
            }
            _connector.Close();
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
    }
}