# *PriorityMap* Documentation

# This documentation provides information about the new data structure called PriorityMap.
-----------------------------------------------------------------------------------------
## *_priorityMap* Property
### Type: SortedDictionary
### Description: A sorted dictionary of data structures. The key represents the priority of the date structure. Higher number -> higher priority.
-----------------------------------------------------------------------------------------
## *Count* Property
### Type: int
### Returns: The total number of elements in the PriorityMap.
### Description: Gets the total number of elements in the PriorityMap.
-----------------------------------------------------------------------------------------

## *Add* Method

**Signature:**
```csharp
public void Add(T element, int priority)
```
### Parameters:
- element: The element to add.
- priority: The priority associated with the element (lower values indicate higher priority).

### Description: Adds an element with the specified priority to the PriorityMap.
  
### Exceptions: System.InvalidOperationException - Thrown when the specified element does not exist within the PriorityMap.
-----------------------------------------------------------------------------------------
## *AddList* Method

**Signature:**
```csharp
public bool AddList(List<T> list, int priority)
```

### Parameters:
- list: The list to add to the PriorityMap.
- priority: The priority associated with the list.

### Returns: 
true if the list is added successfully. false if a list with the same priority already exists.

### Description: Adds a list to the PriorityMap with the specified priority. Returns true if added successfully, false if a list with the same priority already exists.
-----------------------------------------------------------------------------------------
## *RemoveHighestPriority* Method

**Signature:**
```csharp
public T RemoveHighestPriority()
```

### Returns: 
The element with the highest priority.

### Exceptions: System.InvalidOperationException - Thrown when the PriorityMap is empty.

### Description: Removes and returns the element with the highest priority from the PriorityMap.
-----------------------------------------------------------------------------------------
## *IsEmpty* Method

**Signature:**
```csharp
public bool IsEmpty()
```

### Returns: 
true if the PriorityMap is empty, false if the PriorityMap contains elements.

### Description: Checks if the PriorityMap is empty.
-----------------------------------------------------------------------------------------
## *IsExist* Method

**Signature:**
```csharp
public bool IsExist(T element)
```

### Parameters: 
- element: The element to check for existence.

### Returns: 
true if the element exists in the PriorityMap. false if the element is not found.

### Description: Checks if the specified element exists in the PriorityMap.
-----------------------------------------------------------------------------------------
## *GetPriority* Method

**Signature:**
```csharp
public int GetPriority(T element)
```

### Parameters:
- element: The element to retrieve the priority for.

### Returns:
The priority of the element if found. -1 if the element is not found.

### Description: Gets the priority of the specified element in the PriorityMap.
-----------------------------------------------------------------------------------------
## *UpdatePriority* Method

**Signature:**
```csharp
public void UpdatePriority(T element, int newPriority)
```

### Parameters:
- element: The element whose priority needs to be updated.
- newPriority: The new priority to assign to the element.

### Exceptions: System.InvalidOperationException - Thrown when the specified element does not exist within the PriorityMap.

### Description: Updates the priority of the specified element in the PriorityMap.
-----------------------------------------------------------------------------------------
## *TryPeekHighestPriority* Method

**Signature:**
```csharp
public T? TryPeekHighestPriority()
```

### Returns:
The element with the highest priority if the highest-priority list is not empty. null if the PriorityMap is empty.

### Exceptions: System.InvalidOperationException - Thrown when the PriorityMap is empty.

### Description: Retrieves the element with the highest priority from the PriorityMap without removing it.
-----------------------------------------------------------------------------------------
## *RemoveHighestPriorityList* Method

**Signature:**
```csharp
public List<T> RemoveHighestPriorityList()
```

### Returns:
The list with the highest priority if the PriorityMap is not empty.

### Exceptions: System.InvalidOperationException - Thrown if the PriorityMap is empty or if there is an issue retrieving the highest priority list.

### Description: Removes and returns the list with the highest priority from the PriorityMap.
-----------------------------------------------------------------------------------------

## *Reset* Method

**Signature:**
```csharp
public void Reset()
```

### Description: Resets the PriorityMap by creating a new instance.
-----------------------------------------------------------------------------------------
## *Clear* Method

**Signature:**
```csharp
public void Clear()
```

### Description: Clears the PriorityMap by resetting the original instance.
-----------------------------------------------------------------------------------------
## *ToArray* Method

**Signature:**
```csharp
public T[] ToArray()
```

### Returns: 
An array containing all elements in the PriorityMap.

### Description: Converts all elements in the PriorityMap to a flat array.
-----------------------------------------------------------------------------------------
## *ToListArray* Method

**Signature:** 
```csharp
public List<T>[] ToListArray()
```

### Returns: 
An array of lists containing the elements from the PriorityMap.

### Description: Converts the PriorityMap to an array of lists containing the elements.
-----------------------------------------------------------------------------------------
## *GetAllPriorities* Method

**Signature:** 
```csharp
public IEnumerable<int> GetAllPriorities()
```

### Returns:
An IEnumerable<int> containing all priorities in the PriorityMap.

### Exceptions: System.InvalidOperationException - Thrown when the PriorityMap is empty.

### Description: Retrieves all priorities from the PriorityMap.
-----------------------------------------------------------------------------------------
## *RemoveElement* Method

**Signature:**
```csharp
public bool RemoveElement(T element)
```

### Parameters:
- element: The element to be removed from all lists within the PriorityMap.

### Returns:
true if the operation is successful. false if the element is not found or the PriorityMap is empty.

### Description: Removes all occurrences of a specific element from the PriorityMap.
-----------------------------------------------------------------------------------------
## *ClearPriority* Method

**Signature:**
```csharp
public bool ClearPriority(int priority)
```

### Parameters:
- priority: The priority of the list to be cleared.
  
### Returns:
true if the list with the specified priority exists and is successfully cleared. false if the list with the specified priority does not exist or the PriorityMap is empty.\

### Description: Clears the list with the specified priority from the PriorityMap.
-----------------------------------------------------------------------------------------
## *GetElementsWithPriority* Method

**Signature:** 
```csharp
public IEnumerable<T> GetElementsWithPriority(int priority)
```

### Parameters:
- priority: The priority of the elements to retrieve.

### Returns:
An IEnumerable<T> containing all elements with the specified priority.

### Description: Retrieves all elements with the specified priority from the PriorityMap.
-----------------------------------------------------------------------------------------
## *MovePriority* Method

**Signature:**
```csharp
public void MovePriority(int sourcePriority, int destinationPriority)
```

### Parameters:
- sourcePriority: The source priority.
- destinationPriority: The destination priority.

### Description: Moves all elements from one priority to another in the PriorityMap.
-----------------------------------------------------------------------------------------
## *MergePriorities* Method

**Signature:**
```csharp
public void MergePriorities(int[] priorities, int destinationPriority)
```

### Parameters:
- priorities: An array of priorities to merge.
- destinationPriority: The priority to merge the elements into.

### Description: Merges multiple priorities into a single priority in the PriorityMap.
-----------------------------------------------------------------------------------------
## *MergeDataStructures* Method

**Signature:**
```csharp
public PriorityMap<T> MergeDataStructures(Dictionary<int, List<T>> dataStructures, List<int> priorities)
```
### Parameters:
- dataStructures: A data structure of data structures with values to be merged into the PriorityMap.
- priorities: A data structure of integers representing the priorities for the lists in the PriorityMap.
### Returns:
A new PriorityMap containing the merged data structures.
### Exceptions: System.InvalidOperationException - Thrown when the number of priorities is greater than the number of data structures.

### Description: Merges a data structure of data structures with values and a data structure of integers into a single PriorityMap. The integers represent the priorities, and the data structure represents the lists.
-----------------------------------------------------------------------------------------
