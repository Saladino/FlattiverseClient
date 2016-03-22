using System.Collections.Generic;
using System.Threading;
using Flattiverse;

namespace FlattiverseClient
{
    class Map
    {
        Dictionary<string, Unit> mapUnits = new Dictionary<string, Unit>();
        ReaderWriterLock listLock = new ReaderWriterLock();

        public void Insert(List<Unit> units)
        {
            listLock.AcquireWriterLock(100);
            foreach (Unit u in units)
            {
                mapUnits[u.Name] = u;
            }
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