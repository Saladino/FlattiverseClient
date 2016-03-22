using System.Collections.Generic;
using System.Threading;
using Flattiverse;

namespace FlattiverseClient
{
    class Map
    {
        Dictionary<string, Unit> mapUnits = new Dictionary<string, Unit>();
        ReaderWriterLock listLock = new ReaderWriterLock();
        private int _tick = 0;

        public void Insert(List<Unit> units)
        {
            listLock.AcquireWriterLock(100);
            List<string> outdatedMapUnits = new List<string>();

            //populate with new data
            foreach (Unit u in units)
            {
                u.Tag = new Tag() { ScannedAt = (long) _tick};
                mapUnits[u.Name] = u;
            }

            //get outdated data
            foreach (var mapUnit in mapUnits.Values)
            {
                if (_tick - ((Tag)mapUnit.Tag).ScannedAt > 8)
                {
                    outdatedMapUnits.Add(mapUnit.Name);
                }
            }

            //delete outdated data
            foreach (var outdatedMapUnit in outdatedMapUnits)
            {
                mapUnits.Remove(outdatedMapUnit);
            }
            _tick++;
            listLock.ReleaseWriterLock();
        }

        public List<Unit> Units
        {
            get
            {
                listLock.AcquireReaderLock(100);
                List<Unit> units = new List<Unit>(mapUnits.Values);
                listLock.ReleaseReaderLock(); return units;
            }
        }
    }
}