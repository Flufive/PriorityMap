using Newtonsoft.Json;

namespace PriorityMap
{
    public class PriorityMap<T>
    {
        private SortedDictionary<int, List<T>> _priorityMap;
        private readonly object _lock = new object();
        public int Count
        {
            get
            {
                int totalCount = 0;

                foreach (var kvp in _priorityMap)
                {
                    totalCount += kvp.Value.Count;
                }

                return totalCount;
            }
        }
        public PriorityMap(IComparer<int>? priorityComparer = null)
        {
            _priorityMap = new SortedDictionary<int, List<T>>(priorityComparer);
        }
        public T GetElementAt(int priority, int index)
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't get any elements.");
                }
                if (!_priorityMap.ContainsKey(priority))
                {
                    throw new ArgumentOutOfRangeException($"Priority {priority} doesn't exist within PriorityMap.");
                }
                if (_priorityMap.TryGetValue(priority, out var priorityList) && index < priorityList.Count)
                {
                    return priorityList[index];
                }
                throw new InvalidOperationException("Achievement unlocked: How did we get here?");
            }
        }
        public void Add(T element, int priority)
        {
            lock (_lock)
            {
                if (!_priorityMap.TryGetValue(priority, out var list))
                {
                    list = new List<T>();
                    _priorityMap[priority] = list;
                }

                list.Add(element);
            }
        }
        public bool AddList(IEnumerable<T> list, int priority)
        {
            lock (_lock)
            {
                if (!_priorityMap.ContainsKey(priority))
                {
                    _priorityMap[priority] = list.ToList();
                    return true;
                }
                return false;
            }
        }
        public T RemoveHighestPriority()
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove highest priority");
                }
                var highestPriority = _priorityMap.Keys.Last();
                var highestPriorityList = _priorityMap[highestPriority];
                var element = highestPriorityList[0];
                highestPriorityList.RemoveAt(0);
                if (highestPriorityList.Count == 0)
                {
                    _priorityMap.Remove(highestPriority);
                }
                return element;
            }
        }
        public bool IsEmpty()
        {
            lock (_lock)
            {
                if (_priorityMap.Count == 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool IsExist(T element)
        {
            lock (_lock)
            {
                foreach (var kvp in _priorityMap)
                {
                    var elements = kvp.Value;

                    if (elements.Contains(element))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public int GetPriority(T element)
        {
            lock (_lock)
            {
                foreach (var kvp in _priorityMap)
                {
                    var priority = kvp.Key;
                    var elements = kvp.Value;

                    if (elements.Contains(element))
                    {
                        return priority;
                    }
                }
                return -1;
            }
        }
        public void UpdatePriority(T element, int newPriority)
        {
            lock (_lock)
            {
                if (!IsExist(element))
                {
                    throw new InvalidOperationException("Object does not exist within PriorityMap.");
                }
                var currentPriority = GetPriority(element);
                _priorityMap[currentPriority].Remove(element);
                if (!_priorityMap.ContainsKey(newPriority))
                {
                    _priorityMap[newPriority] = new List<T>();
                }
                _priorityMap[newPriority].Add(element);
            }
        }
        public T? TryPeekHighestPriority()
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty.");
                }
                var highestPriority = _priorityMap.Keys.Last();
                var highestPriorityList = _priorityMap[highestPriority];
                if (highestPriorityList.Count == 0)
                {
                    return default(T);
                }
                var highestPriorityElement = highestPriorityList[0];
                return highestPriorityElement;
            }
        }
        public List<T> RemoveHighestPriorityList()
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove highest priority list.");
                }
                var highestPriority = _priorityMap.Keys.Last();
                if (_priorityMap.TryGetValue(highestPriority, out var highestPriorityList))
                {
                    _priorityMap.Remove(highestPriority);
                    return highestPriorityList;
                }
                else
                {
                    throw new InvalidOperationException("Failed to retrieve the highest priority list.");
                }
            }
        }
        public void Reset()
        {
            lock (_lock)
            {
                _priorityMap = new SortedDictionary<int, List<T>>();
            }
        }
        public void Clear()
        {
            lock (_lock)
            {
                _priorityMap.Clear();
            }
        }
        public T[] ToArray()
        {
            lock (_lock)
            {
                List<T> allElements = new List<T>();

                foreach (var kvp in _priorityMap)
                {
                    allElements.AddRange(kvp.Value);
                }

                return allElements.ToArray();
            }
        }
        public List<T>[] ToListArray()
        {
            lock (_lock)
            {
                return _priorityMap.Values.ToArray();
            }
        }
        public IEnumerable<int> GetAllPriorities()
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't get priorities.");
                }
                List<int> priorities = new List<int>();
                foreach (var kvp in _priorityMap)
                {
                    priorities.Add(kvp.Key);
                }
                return priorities;
            }
        }
        public bool RemoveElement(T element)
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove any elements.");
                }
                else if (!IsExist(element))
                {
                    return false;
                }

                foreach (var kvp in _priorityMap)
                {
                    kvp.Value.RemoveAll(item => EqualityComparer<T>.Default.Equals(item, element));
                }
                return true;
            }
        }
        public bool ClearPriority(int priority)
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove list.");
                }
                if (_priorityMap.ContainsKey(priority))
                {
                    _priorityMap[priority].Clear();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public IEnumerable<T> GetElementsWithPriority(int priority)
        {
            lock (_lock)
            {
                if (_priorityMap.TryGetValue(priority, out var priorityList))
                {
                    return priorityList;
                }
                return Enumerable.Empty<T>();
            }
        }
        public IEnumerable<T> GetAllElements()
        {
            lock (_lock)
            {
                return _priorityMap.Values.SelectMany(list => list);
            }
        }
        public string Serialize()
        {
            lock (_lock)
            {
                return JsonConvert.SerializeObject(this);
            }
        }
        public PriorityMap<T>? Deserialize(string json)
        {
            lock (_lock)
            {
                return JsonConvert.DeserializeObject<PriorityMap<T>>(json);
            }
        }
        public void MovePriority(int sourcePriority, int destinationPriority)
        {
            lock (_lock)
            {
                if (_priorityMap.TryGetValue(sourcePriority, out var sourceList))
                {
                    if (!_priorityMap.ContainsKey(destinationPriority))
                    {
                        _priorityMap[destinationPriority] = new List<T>();
                    }

                    _priorityMap[destinationPriority].AddRange(sourceList);
                    sourceList.Clear();

                    if (sourceList.Count == 0)
                    {
                        _priorityMap.Remove(sourcePriority);
                    }
                }
            }
        }
        public void MergePriorities(int[] priorities, int destinationPriority)
        {
            lock (_lock)
            {
                if (priorities.Length == 0)
                {
                    return;
                }

                foreach (var priority in priorities)
                {
                    if (_priorityMap.TryGetValue(priority, out var sourceList))
                    {
                        if (!_priorityMap.ContainsKey(destinationPriority))
                        {
                            _priorityMap[destinationPriority] = new List<T>();
                        }

                        _priorityMap[destinationPriority].AddRange(sourceList);
                        sourceList.Clear();

                        if (sourceList.Count == 0)
                        {
                            _priorityMap.Remove(priority);
                        }
                    }
                }
            }
        }
        public PriorityMap<T> MergeDataStructures(Dictionary<int, List<T>> dataStructures, List<int> priorities)
        {
            lock (_lock)
            {
                if (priorities.Count > dataStructures.Count)
                {
                    throw new InvalidOperationException("Number of priorities cannot be greater than the number of data structures.");
                }
                var mergedPriorityMap = new PriorityMap<T>();
                for (int i = 0; i < priorities.Count; i++)
                {
                    int priority = priorities[i];
                    List<T> dataList = dataStructures.TryGetValue(i, out var value) ? value : new List<T>();
                    mergedPriorityMap.AddList(dataList, priority);
                }
                return mergedPriorityMap;
            }
        }
        public void MergePrioritiesWithCustomLogic(int[] priorities, int destinationPriority, Func<List<T>, List<T>, List<T>> mergeFunction)
        {
            foreach (var priority in priorities)
            {
                if (_priorityMap.TryGetValue(priority, out var sourceList))
                {
                    if (!_priorityMap.ContainsKey(destinationPriority))
                    {
                        _priorityMap[destinationPriority] = new List<T>();
                    }
                    _priorityMap[destinationPriority] = mergeFunction(_priorityMap[destinationPriority], sourceList);
                    sourceList.Clear();
                    if (sourceList.Count == 0)
                    {
                        _priorityMap.Remove(priority);
                    }
                }
            }
        }
    }
}
