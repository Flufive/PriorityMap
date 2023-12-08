using Newtonsoft.Json;

namespace PriorityMap
{
    /// <summary>
    /// Represents a data structure that combines the characteristics of a priority queue and a key-value map.
    /// Elements are associated with priorities, allowing efficient retrieval, update, and removal based on priority.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the PriorityMap.</typeparam>
    public class PriorityMap<T>
    {
        /// <summary>
        /// A sorted dictionary of data structures. The key represents the priority of the date structure. Higher number -> higher priority.
        /// </summary>
        private SortedDictionary<int, List<T>> _priorityMap;

        /// <summary>
        /// A lock object used for synchronization to ensure thread safety in critical sections.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Gets the total number of elements in the PriorityMap.
        /// </summary>
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

        /// <summary>
        /// Gets a read-only collection of all elements in the priority map.
        /// </summary>
        public IReadOnlyCollection<T> AllElements
        {
            get
            {
                lock (_lock)
                {
                    return _priorityMap.Values.SelectMany(list => list).ToList().AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityMap{T}"/> class.
        /// </summary>
        /// <param name="priorityComparer">
        /// An optional comparer to use when sorting priorities in the internal <see cref="SortedDictionary{TKey, TValue}"/>.
        /// If not specified (null), the default comparer for the key type <see cref="int"/> will be used.
        /// </param>
        public PriorityMap(IComparer<int>? priorityComparer = null)
        {
            _priorityMap = new SortedDictionary<int, List<T>>(priorityComparer);
        }

        /// <summary>
        /// Retrieves the element at the specified index within the list associated with the given priority in the PriorityMap.
        /// </summary>
        /// <param name="priority">The priority of the list containing the desired element.</param>
        /// <param name="index">The index of the element within the specified priority list.</param>
        /// <returns>The element at the specified index within the priority list.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the PriorityMap is empty or if there is an issue retrieving the specified element.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified priority does not exist within the PriorityMap.
        /// </exception>
        public T GetElementAt(int priority, int index)
        {
            lock (_lock)
            {
                // Make sure PriorityMap isn't empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't get any elements.");
                }

                // Make sure priority exists
                if (!_priorityMap.ContainsKey(priority))
                {
                    throw new ArgumentOutOfRangeException($"Priority {priority} doesn't exist within PriorityMap.");
                }

                // Retrieve element
                if (_priorityMap.TryGetValue(priority, out var priorityList) && index < priorityList.Count)
                {
                    return priorityList[index];
                }

                // We shouldn't be here even
                throw new InvalidOperationException("Achievement unlocked: How did we get here?");
            }
        }

        /// <summary>
        /// Adds an element with the specified priority to the PriorityMap.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="priority">The priority associated with the element (lower values indicate higher priority).</param>
        public void Add(T element, int priority)
        {
            lock (_lock)
            {
                if (!_priorityMap.TryGetValue(priority, out var list))
                {
                    list = new List<T>();
                    _priorityMap[priority] = list;
                }

                list.Add(element); // Add the element to the corresponding priority list
            }
        }

        /// <summary>
        /// Adds multiple items with their corresponding priorities to the PriorityMap.
        /// </summary>
        /// <param name="items">A dictionary where keys are items and values are their priorities.</param>
        public void BulkAdd(IDictionary<T, int> items)
        {
            foreach (var kvp in items)
            {
                Add(kvp.Key, kvp.Value);
            }
        }
    
        /// <summary>
        /// Creates a deep copy of the PriorityMap.
        /// </summary>
        /// <remarks>
        /// The new PriorityMap has distinct instances of lists and items.
        /// </remarks>
        /// <returns>A new PriorityMap instance that is a deep copy of the original.</returns>
        public PriorityMap<T> Clone()
        {
            var clone = new PriorityMap<T>();
            
            foreach (var kvp in priorityMap)
            {
                // Create a new list and add copies of the items
                clone.priorityMap[kvp.Key] = new List<T>(kvp.Value.Select(item => item));
            }
        
            return clone;
        }

    
        /// <summary>
        /// Retrieves items with priorities within the specified range.
        /// </summary>
        /// <param name="minPriority">The minimum priority (inclusive).</param>
        /// <param name="maxPriority">The maximum priority (inclusive).</param>
        /// <returns>An IEnumerable containing items within the specified priority range.</returns>
        public IEnumerable<T> PriorityRange(int minPriority, int maxPriority)
        {
            var selectedItems = priorityMap
                .Where(kvp => kvp.Key >= minPriority && kvp.Key <= maxPriority)
                .SelectMany(kvp => kvp.Value);
            return selectedItems;
        }

        /// <summary>
        /// Adds a list to the PriorityMap with the specified priority.
        /// If a list with the same priority already exists, returns false; otherwise, returns true.
        /// </summary>
        /// <param name="list">The list to add to the PriorityMap.</param>
        /// <param name="priority">The priority associated with the list.</param>
        /// <returns>True if the list is added successfully; otherwise, false.</returns>
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

        /// <summary>
        /// Removes and returns the element with the highest priority from the PriorityMap.
        /// If multiple elements share the highest priority, the first one added will be removed.
        /// </summary>
        /// <returns>The element with the highest priority.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the PriorityMap is empty.</exception>
        public T RemoveHighestPriority()
        {
            lock (_lock)
            {
                // Check if map is empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove highest priority");
                }

                // Find the highest priority
                var highestPriority = _priorityMap.Keys.Last();

                // Retrieve the list of elements with the highest priority
                var highestPriorityList = _priorityMap[highestPriority];

                // Get the first element from the list (FIFO within the same priority)
                var element = highestPriorityList[0];

                // Remove the element from the list
                highestPriorityList.RemoveAt(0);

                // Remove the priority entry if the list is empty
                if (highestPriorityList.Count == 0)
                {
                    _priorityMap.Remove(highestPriority);
                }

                // Return the removed element
                return element;
            }
        }

        /// <summary>
        /// Checks if the priorityMap is empty
        /// </summary>
        /// <returns>True if map is empty, false if not</returns>
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

        /// <summary>
        /// Checks if the specified element exists in the PriorityMap.
        /// </summary>
        /// <param name="element">The element to check for existence.</param>
        /// <returns>True if the element exists in the PriorityMap; otherwise, false.</returns>
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
                // Element not found in the PriorityMap
                return false;
            }
        }

        /// <summary>
        /// Gets the priority of the specified element in the PriorityMap.
        /// </summary>
        /// <param name="element">The element to retrieve the priority for.</param>
        /// <returns>The priority of the element if found; otherwise, returns -1.</returns>
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

        /// <summary>
        /// Updates the priority of the specified element in the PriorityMap.
        /// </summary>
        /// <param name="element">The element whose priority needs to be updated.</param>
        /// <param name="newPriority">The new priority to assign to the element.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified element does not exist within the PriorityMap.
        /// </exception>
        public void UpdatePriority(T element, int newPriority)
        {
            lock (_lock)
            {
                // Check if the element exists in the PriorityMap
                if (!IsExist(element))
                {
                    throw new InvalidOperationException("Object does not exist within PriorityMap.");
                }

                // Retrieve the current priority of the element
                var currentPriority = GetPriority(element);

                // Remove the element from its current priority
                _priorityMap[currentPriority].Remove(element);

                // Add the element to the new priority
                if (!_priorityMap.ContainsKey(newPriority))
                {
                    _priorityMap[newPriority] = new List<T>();
                }
                _priorityMap[newPriority].Add(element);
            }
        }

        /// <summary>
        /// Retrieves the element with the highest priority from the PriorityMap without removing it.
        /// </summary>
        /// if the highest-priority list is not empty; otherwise, an invalid result.
        /// <returns>True if the highest-priority list is not empty; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the PriorityMap is empty.</exception>
        public T? TryPeekHighestPriority()
        {
            lock (_lock)
            {
                // Check if the PriorityMap is empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty.");
                }

                // Find the highest priority
                var highestPriority = _priorityMap.Keys.Last();

                // Retrieve the list of elements with the highest priority
                var highestPriorityList = _priorityMap[highestPriority];

                // Check if the highest-priority list is empty
                if (highestPriorityList.Count == 0)
                {
                    // Return an invalid result
                    return default(T);
                }

                // Retrieve and return the element with the highest priority
                var highestPriorityElement = highestPriorityList[0];
                return highestPriorityElement;
            }
        }

        /// <summary>
        /// Removes and returns the list with the highest priority from the PriorityMap.
        /// </summary>
        /// <returns>
        /// The list with the highest priority if the PriorityMap is not empty;
        /// otherwise, throws an InvalidOperationException if the PriorityMap is empty
        /// or if there is an issue retrieving the highest priority list.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the PriorityMap is empty or if there is an issue retrieving the highest priority list.</exception>
        public List<T> RemoveHighestPriorityList()
        {
            lock (_lock)
            {
                // Check if the PriorityMap is empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove highest priority list.");
                }

                // Get the highest priority
                var highestPriority = _priorityMap.Keys.Last();

                // Retrieve and remove the list with the highest priority
                if (_priorityMap.TryGetValue(highestPriority, out var highestPriorityList))
                {
                    _priorityMap.Remove(highestPriority);
                    return highestPriorityList;
                }
                else
                {
                    // This should not happen if the map is consistent, but handle it gracefully
                    throw new InvalidOperationException("Failed to retrieve the highest priority list.");
                }
            }
        }

        /// <summary>
        /// Resets the PriorityMap by creating a new instance.
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _priorityMap = new SortedDictionary<int, List<T>>();
            }
        }

        /// <summary>
        /// Clears the PriorityMap by resetting the original instance.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _priorityMap.Clear();
            }
        }

        /// <summary>
        /// Converts all elements in the PriorityMap to a flat array.
        /// </summary>
        /// <returns>An array containing all elements in the PriorityMap.</returns>
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

        /// <summary>
        /// Converts the PriorityMap to an array of lists containing the elements.
        /// </summary>
        /// <returns>An array of lists containing the elements from the PriorityMap.</returns>
        public List<T>[] ToListArray()
        {
            lock (_lock)
            {
                return _priorityMap.Values.ToArray();
            }
        }

        /// <summary>
        /// Retrieves all priorities from the PriorityMap.
        /// </summary>
        /// <returns>
        /// An IEnumerable<int> containing all priorities in the PriorityMap.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when PriorityMap is empty</exception>
        public IEnumerable<int> GetAllPriorities()
        {
            lock (_lock)
            {
                // Check if PriorityMap is empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't get priorities.");
                }
                // Create a list to store priorities
                List<int> priorities = new List<int>();

                // Iterate over the keys of the _priorityMap and add them to the list
                foreach (var kvp in _priorityMap)
                {
                    priorities.Add(kvp.Key);
                }

                // Return the list of priorities as IEnumerable<int>
                return priorities;
            }
        }

        /// <summary>
        /// Checks if the priority map contains any duplicate elements.
        /// </summary>
        /// <returns>True if there are duplicates; otherwise, false.</returns>
        public bool ContainsDuplicates()
        {
            lock (_lock)
            {
                var allElements = AllElements.ToList();
                var distinctElements = new HashSet<T>(allElements, EqualityComparer<T>.Default);
                return allElements.Count != distinctElements.Count;
            }
        }

        /// <summary>
        /// Retrieves the total count of unique elements in the priority map.
        /// </summary>
        /// <returns>The count of unique elements.</returns>
        public int GetUniqueElementCount()
        {
            lock (_lock)
            {
                var allElements = AllElements.ToList();
                var distinctElements = new HashSet<T>(allElements, EqualityComparer<T>.Default);
                return distinctElements.Count;
            }
        }

        /// <summary>
        /// Retrieves the elements with the highest priority.
        /// </summary>
        /// <returns>A list of elements with the highest priority.</returns>
        public List<T> GetElementsWithHighestPriority()
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't retrieve elements with the highest priority.");
                }

                var highestPriority = _priorityMap.Keys.Last();
                return _priorityMap[highestPriority].ToList();
            }
        }

        /// <summary>
        /// Splits the priority map into two maps based on a predicate.
        /// </summary>
        /// <param name="predicate">The predicate to determine which priority map the element should belong to.</param>
        /// <returns>
        /// A tuple containing two PriorityMap instances:
        /// satisfying - PriorityMap with elements satisfying the predicate.
        /// notSatisfying - PriorityMap with elements not satisfying the predicate.
        /// </returns>
        public (PriorityMap<T> satisfying, PriorityMap<T> notSatisfying) SplitByPredicate(Func<T, bool> predicate)
        {
            lock (_lock)
            {
                var satisfyingMap = new PriorityMap<T>();
                var notSatisfyingMap = new PriorityMap<T>();

                if (IsEmpty())
                {
                    return (satisfyingMap, notSatisfyingMap);
                }

                foreach (var kvp in _priorityMap)
                {
                    var satisfyingElements = kvp.Value.Where(predicate).ToList();
                    var notSatisfyingElements = kvp.Value.Except(satisfyingElements, EqualityComparer<T>.Default).ToList();

                    if (satisfyingElements.Count > 0)
                        satisfyingMap.AddList(satisfyingElements, kvp.Key);

                    if (notSatisfyingElements.Count > 0)
                        notSatisfyingMap.AddList(notSatisfyingElements, kvp.Key);
                }

                return (satisfyingMap, notSatisfyingMap);
            }
        }

        /// <summary>
        /// Executes a specified action on each element in the priority map.
        /// </summary>
        /// <param name="action">The action to perform on each element.</param>
        public void ForEachElement(Action<T> action)
        {
            lock (_lock)
            {
                foreach (var priorityList in _priorityMap.Values)
                {
                    foreach (var element in priorityList)
                    {
                        action(element);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a dictionary with priorities as keys and the number of elements in each priority list.
        /// </summary>
        /// <returns>A dictionary with priorities and the corresponding count of elements.</returns>
        public Dictionary<int, int> GetPriorityCounts()
        {
            lock (_lock)
            {
                return _priorityMap.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
            }
        }

        /// <summary>
        /// Reverses the order of elements within each priority list.
        /// </summary>
        public void ReverseElementOrder()
        {
            lock (_lock)
            {
                foreach (var priorityList in _priorityMap.Values)
                {
                    priorityList.Reverse();
                }
            }
        }

        /// <summary>
        /// Checks if the priority map contains elements at the specified priority.
        /// </summary>
        /// <param name="priority">The priority to check.</param>
        /// <returns>True if the priority map contains elements at the specified priority; otherwise, false.</returns>
        public bool HasElementsAtPriority(int priority)
        {
            lock (_lock)
            {
                return DoesPriorityExist(priority) && _priorityMap[priority].Count > 0;
            }
        }

        /// <summary>
        /// Removes all occurrences of a specific element from the PriorityMap.
        /// </summary>
        /// <returns>True if operation is successful, otherwise false</returns>
        /// <param name="element">The element to be removed from all lists within the PriorityMap.</param>
        /// <exception cref="InvalidOperationException">Thrown when PriorityMap is empty</exception>
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

        /// <summary>
        /// Clears the list with the specified priority from the PriorityMap.
        /// </summary>
        /// <param name="priority">The priority of the list to be cleared.</param>
        /// <returns>
        ///   <c>true</c> if the list with the specified priority exists and is successfully cleared;
        ///   otherwise, <c>false</c> if the list with the specified priority does not exist.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if the PriorityMap is empty.</exception>
        public bool ClearPriority(int priority)
        {
            lock (_lock)
            {
                // Check if the PriorityMap is empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove list.");
                }

                // Check if the specified priority exists in the map
                if (_priorityMap.ContainsKey(priority))
                {
                    // List with the specified priority exists, clear it
                    _priorityMap[priority].Clear();
                    return true;
                }
                else
                {
                    // List with the specified priority doesn't exist, operation failed
                    return false;
                }
            }
        }

        /// <summary>
        /// Retrieves all elements with the specified priority from the PriorityMap.
        /// </summary>
        /// <param name="priority">The priority of the elements to retrieve.</param>
        /// <returns>An IEnumerable&lt;T&gt; containing all elements with the specified priority.</returns>
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

        /// <summary>
        /// Retrieves all elements from the PriorityMap, combining them into a single sequence.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all elements in the PriorityMap.</returns>
        public IEnumerable<T> GetAllElements()
        {
            lock (_lock)
            {
                return _priorityMap.Values.SelectMany(list => list);
            }
        }

        /// <summary>
        /// Serializes the PriorityMap to a JSON string using the Newtonsoft.Json library.
        /// </summary>
        /// <returns>A JSON string representing the serialized PriorityMap.</returns>
        public string Serialize()
        {
            lock (_lock)
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        /// <summary>
        /// Deserializes a JSON string into a PriorityMap using the Newtonsoft.Json library.
        /// </summary>
        /// <param name="json">The JSON string to deserialize into a PriorityMap.</param>
        /// <returns>A new instance of PriorityMap with data from the deserialized JSON string.</returns>
        public PriorityMap<T>? Deserialize(string json)
        {
            lock (_lock)
            {
                return JsonConvert.DeserializeObject<PriorityMap<T>>(json);
            }
        }

        /// <summary>
        /// Moves all elements from one priority to another in the PriorityMap.
        /// </summary>
        /// <param name="sourcePriority">The source priority.</param>
        /// <param name="destinationPriority">The destination priority.</param>
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

        /// <summary>
        /// Merges multiple priorities into a single priority in the PriorityMap.
        /// </summary>
        /// <param name="priorities">An array of priorities to merge.</param>
        /// <param name="destinationPriority">The priority to merge the elements into.</param>
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

        /// <summary>
        /// Checks if a specified priority does not exist within the priority map.
        /// </summary>
        /// <param name="priority">The priority to check.</param>
        /// <returns>True if the priority does not exist; otherwise, false.</returns>
        public bool DoesPriorityExist(int priority)
        {
            lock (_lock)
            {
                if (IsEmpty())
                {
                    return false;
                }
                return _priorityMap.ContainsKey(priority);
            }
        }

        /// <summary>
        /// Removes an element at the specified priority and index.
        /// </summary>
        /// <param name="priority">The priority of the element to remove.</param>
        /// <param name="index">The index of the element to remove within its priority list.</param>
        /// <exception cref="InvalidOperationException">Thrown if the priority map is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the specified priority does not exist within the priority map.</exception>
        public void RemoveElementAt(int priority, int index)
        {
            lock (_lock)
            {
                // Check if the priority map is empty
                if (IsEmpty())
                {
                    throw new InvalidOperationException("PriorityMap is empty, can't remove any elements");
                }

                // Check if the specified priority exists within the priority map
                if (!DoesPriorityExist(priority))
                {
                    throw new ArgumentOutOfRangeException($"Priority {priority} doesn't exist within PriorityMap");
                }

                // Retrieve the priority list and remove the element at the specified index
                if (_priorityMap.TryGetValue(priority, out var priorityList) && index < priorityList.Count)
                {
                    priorityList.RemoveAt(index);

                    // Remove the entire priority if the list is empty after removal
                    if (priorityList.Count == 0)
                    {
                        _priorityMap.Remove(priority);
                    }
                }
            }
        }

        /// <summary>
        /// Adds or updates an element in the priority map with the specified priority.
        /// </summary>
        /// <param name="element">The element to add or update.</param>
        /// <param name="priority">The priority of the element.</param>
        public void AddOrUpdate(T element, int priority)
        {
            lock (_lock)
            {
                // Try to get the existing list for the specified priority
                if (_priorityMap.TryGetValue(priority, out var list))
                {
                    // Update existing list
                    if (!list.Contains(element))
                    {
                        list.Add(element);
                    }
                }
                else
                {
                    // Add new priority list
                    Add(element, priority);
                }
            }
        }

        /// <summary>
        /// Adds or updates a collection of elements in the priority map with the specified priority.
        /// </summary>
        /// <param name="elements">The collection of elements to add or update.</param>
        /// <param name="priority">The priority of the elements.</param>
        public void AddOrUpdateList(IEnumerable<T> elements, int priority)
        {
            lock (_lock)
            {
                // Try to get the existing list for the specified priority
                if (_priorityMap.TryGetValue(priority, out var list))
                {
                    // Update existing list
                    list.AddRange(elements.Where(element => !list.Contains(element)));
                }
                else
                {
                    // Add new priority list
                    AddList(elements, priority);
                }
            }
        }

        /// <summary>
        /// Attempts to retrieve the list of elements for a specified priority in the priority map.
        /// </summary>
        /// <param name="priority">The priority of the elements to retrieve.</param>
        /// <param name="priorityList">When this method returns, contains the list of elements for the specified priority, if the priority exists; otherwise, null.</param>
        /// <returns>True if the priority exists; otherwise, false.</returns>
        public bool TryGetPriorityList(int priority, out List<T>? priorityList)
        {
            lock (_lock)
            {
                // Check if the specified priority exists
                if (!DoesPriorityExist(priority))
                {
                    priorityList = null;
                    return false;
                }

                // Retrieve the priority list
                return _priorityMap.TryGetValue(priority, out priorityList);
            }
        }

        /// <summary>
        /// Trims excess capacity from the lists of elements in the priority map.
        /// </summary>
        public void TrimExcess()
        {
            lock (_lock)
            {
                // Iterate through all priority lists and trim excess capacity
                foreach (var priorityList in _priorityMap.Values)
                {
                    priorityList.TrimExcess();
                }
            }
        }

        /// <summary>
        /// Checks if the priority map contains elements at all specified priorities.
        /// </summary>
        /// <param name="priorities">The priorities to check.</param>
        /// <returns>True if the priority map contains elements at all specified priorities; otherwise, false.</returns>
        public bool HasElementsAtAllPriorities(params int[] priorities)
        {
            lock (_lock)
            {
                return priorities.All(DoesPriorityExist);
            }
        }

        /// <summary>
        /// Returns the minimum count of elements among all priority lists.
        /// </summary>
        /// <returns>The minimum count of elements among all priority lists.</returns>
        public int GetMinElementsCount()
        {
            lock (_lock)
            {
                return _priorityMap.Values.Min(list => list.Count);
            }
        }

        /// <summary>
        /// Returns the maximum count of elements among all priority lists.
        /// </summary>
        /// <returns>The maximum count of elements among all priority lists.</returns>
        public int GetMaxElementsCount()
        {
            lock (_lock)
            {
                return _priorityMap.Values.Max(list => list.Count);
            }
        }

        /// <summary>
        /// Merges a data structure of data structures with values and a data structure of integers into a single PriorityMap.
        /// The integers represent the priorities (keys), and the data structure of data structures represents the lists (PriorityMap[key]).
        /// The lists will be sorted in a way that the first list ([0]) will be given the priority of the first integer.
        /// Lists without a corresponding value will be assigned a priority value of 0, giving them low priority.
        /// If the data structure of integers is larger than the other parameter, an exception will be thrown.
        /// </summary>
        /// <param name="dataStructures">A data structure of data structures with values to be merged into the PriorityMap.</param>
        /// <param name="priorities">A data structure of integers representing the priorities for the lists in the PriorityMap.</param>
        /// <returns>A new PriorityMap containing the merged data structures.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data structure of integers is larger than the data structure of data structures.
        /// </exception>
        public PriorityMap<T> MergeDataStructures(Dictionary<int, List<T>> dataStructures, List<int> priorities)
        {
            lock (_lock)
            {
                // Check if the number of priorities matches the number of data structures
                if (priorities.Count > dataStructures.Count)
                {
                    throw new InvalidOperationException("Number of priorities cannot be greater than the number of data structures.");
                }

                // Create a new PriorityMap to store the merged data structures
                var mergedPriorityMap = new PriorityMap<T>();

                // Iterate over the priorities and data structures to merge them into the PriorityMap
                for (int i = 0; i < priorities.Count; i++)
                {
                    int priority = priorities[i];
                    List<T> dataList = dataStructures.TryGetValue(i, out var value) ? value : new List<T>();

                    // Add the data structure to the PriorityMap with the specified priority
                    mergedPriorityMap.AddList(dataList, priority);
                }

                // Return the merged PriorityMap
                return mergedPriorityMap;
            }
        }


        /// <summary>
        /// Merges multiple priorities into a single priority in the PriorityMap using custom logic provided by the specified merge function.
        /// </summary>
        /// <param name="priorities">An array of priorities to merge.</param>
        /// <param name="destinationPriority">The priority to merge the elements into.</param>
        /// <param name="mergeFunction">A custom merge function that defines the logic for combining lists of elements.</param>
        public void MergePrioritiesWithCustomLogic(int[] priorities, int destinationPriority, Func<List<T>, List<T>, List<T>> mergeFunction)
        {
            // Merge priorities using custom logic
            foreach (var priority in priorities)
            {
                // Check if the specified priority exists in the map
                if (_priorityMap.TryGetValue(priority, out var sourceList))
                {
                    // Create the destination list if it doesn't exist
                    if (!_priorityMap.ContainsKey(destinationPriority))
                    {
                        _priorityMap[destinationPriority] = new List<T>();
                    }

                    // Apply the custom merge function to combine the source and destination lists
                    _priorityMap[destinationPriority] = mergeFunction(_priorityMap[destinationPriority], sourceList);

                    // Remove the source priority if its list is empty
                    if (sourceList.Count == 0)
                    {
                        _priorityMap.Remove(priority);
                    }
                }
            }
        }
    }
}
