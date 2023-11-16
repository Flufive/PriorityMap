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

        public PriorityMap()
        {
            _priorityMap = new SortedDictionary<int, List<T>>();
        }

        /// <summary>
        /// Adds an element with the specified priority to the PriorityMap.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="priority">The priority associated with the element (lower values indicate higher priority).</param>
        public void Add(T element, int priority)
        {
            // Add the element to the corresponding priority list
            if (!_priorityMap.TryGetValue(priority, out var list))
            {
                list = new List<T>();
                _priorityMap[priority] = list;
            }

            list.Add(element);
        }

        /// <summary>
        /// Adds a list to the PriorityMap with the specified priority.
        /// If a list with the same priority already exists, returns false; otherwise, returns true.
        /// </summary>
        /// <param name="list">The list to add to the PriorityMap.</param>
        /// <param name="priority">The priority associated with the list.</param>
        /// <returns>True if the list is added successfully; otherwise, false.</returns>
        public bool AddList(List<T> list, int priority)
        {
            if (!_priorityMap.ContainsKey(priority))
            {
                _priorityMap[priority] = list;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes and returns the element with the highest priority from the PriorityMap.
        /// If multiple elements share the highest priority, the first one added will be removed.
        /// </summary>
        /// <returns>The element with the highest priority.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the PriorityMap is empty.</exception>
        public T RemoveHighestPriority()
        {
            // Remove and return the element with the highest priority
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

        /// <summary>
        /// Checks if the priorityMap is empty
        /// </summary>
        /// <returns>True if map is empty, false if not</returns>
        public bool IsEmpty()
        {
            if (_priorityMap.Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the specified element exists in the PriorityMap.
        /// </summary>
        /// <param name="element">The element to check for existence.</param>
        /// <returns>True if the element exists in the PriorityMap; otherwise, false.</returns>
        public bool IsExist(T element)
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


        /// <summary>
        /// Gets the priority of the specified element in the PriorityMap.
        /// </summary>
        /// <param name="element">The element to retrieve the priority for.</param>
        /// <returns>The priority of the element if found; otherwise, returns -1.</returns>
        public int GetPriority(T element)
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

        // PriorityMapResult class to encapsulate the result


        /// <summary>
        /// Retrieves the element with the highest priority from the PriorityMap without removing it.
        /// </summary>
        /// <param name="result">An instance of PriorityMapResult&lt;T&gt; containing the element with the highest priority 
        /// if the highest-priority list is not empty; otherwise, an invalid result.</param>
        /// <returns>True if the highest-priority list is not empty; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the PriorityMap is empty.</exception>
        public T? TryPeekHighestPriority()
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


        /// <summary>
        /// Resets the PriorityMap by creating a new instance.
        /// </summary>
        public void Reset()
        {
            _priorityMap = new SortedDictionary<int, List<T>>();
        }

        /// <summary>
        /// Clears the PriorityMap by resetting the original instance.
        /// </summary>
        public void Clear()
        {
            _priorityMap.Clear();
        }

        /// <summary>
        /// Converts all elements in the PriorityMap to a flat array.
        /// </summary>
        /// <returns>An array containing all elements in the PriorityMap.</returns>
        public T[] ToArray()
        {
            List<T> allElements = new List<T>();

            foreach (var kvp in _priorityMap)
            {
                allElements.AddRange(kvp.Value);
            }

            return allElements.ToArray();
        }

        /// <summary>
        /// Converts the PriorityMap to an array of lists containing the elements.
        /// </summary>
        /// <returns>An array of lists containing the elements from the PriorityMap.</returns>
        public List<T>[] ToListArray()
        {
            return _priorityMap.Values.ToArray();
        }

        /// <summary>
        /// Retrieves all priorities from the PriorityMap.
        /// </summary>
        /// <returns>An IEnumerable<int> containing all priorities in the PriorityMap.</returns>
        /// <exception cref="InvalidOperationException">Thrown when PriorityMap is empty</exception>
        public IEnumerable<int> GetAllPriorities()
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

        /// <summary>
        /// Removes all occurrences of a specific element from the PriorityMap.
        /// </summary>
        /// <returns>True if operation is successful, otherwise false</returns>
        /// <param name="element">The element to be removed from all lists within the PriorityMap.</param>
        /// <exception cref="InvalidOperationException">Thrown when PriorityMap is empty</exception>
        public bool RemoveElement(T element)
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

        /// <summary>
        /// Retrieves all elements with the specified priority from the PriorityMap.
        /// </summary>
        /// <param name="priority">The priority of the elements to retrieve.</param>
        /// <returns>An IEnumerable&lt;T&gt; containing all elements with the specified priority.</returns>
        public IEnumerable<T> GetElementsWithPriority(int priority)
        {
            if (_priorityMap.TryGetValue(priority, out var priorityList))
            {
                return priorityList;
            }
            return Enumerable.Empty<T>();
        }


        /// <summary>
        /// Moves all elements from one priority to another in the PriorityMap.
        /// </summary>
        /// <param name="sourcePriority">The source priority.</param>
        /// <param name="destinationPriority">The destination priority.</param>
        public void MovePriority(int sourcePriority, int destinationPriority)
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


        /// <summary>
        /// Merges multiple priorities into a single priority in the PriorityMap.
        /// </summary>
        /// <param name="priorities">An array of priorities to merge.</param>
        /// <param name="destinationPriority">The priority to merge the elements into.</param>
        public void MergePriorities(int[] priorities, int destinationPriority)
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
}
