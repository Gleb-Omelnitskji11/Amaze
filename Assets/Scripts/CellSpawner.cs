using System.Collections.Generic;
using UnityEngine;

namespace Amaze
{
    public class CellSpawner : MonoBehaviour
    {
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private Transform _parent;

        private readonly List<CellView> _pool = new List<CellView>();

        public void EnsureCapacity(int required)
        {
            while (_pool.Count < required)
            {
                CellView cell = Instantiate(_cellPrefab, Vector3.zero, Quaternion.identity, _parent);
                cell.gameObject.SetActive(false);
                _pool.Add(cell);
            }
        }

        public CellView Get(int index)
        {
            CellView cell = _pool[index];
            cell.gameObject.SetActive(true);
            return cell;
        }

        public void DeactivateRange(int fromIndex)
        {
            for (int i = fromIndex; i < _pool.Count; i++)
            {
                if (_pool[i] != null)
                {
                    _pool[i].Deactivate();
                }
            }
        }

        public void DeactivateAll()
        {
            DeactivateRange(0);
        }
    }
}
