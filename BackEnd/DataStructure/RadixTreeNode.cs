﻿using System.Collections.ObjectModel;

namespace BackEnd.DataStructure;
internal class RadixTreeNode<TKey, TValue> 
	where TKey : notnull
{
    private bool _isValueNode = false;
	public IEnumerable<TKey> Key { get; init; }
    public RadixTreeNodeValue<TValue>? Value { get; init; } = default;
    public ReadOnlyDictionary<TKey, RadixTreeNode<TKey, TValue>> Children { get; init; } = new(new Dictionary<TKey, RadixTreeNode<TKey, TValue>>());
    public bool IsValueNode { get { return _isValueNode; } init { _isValueNode = value; } }
	public RadixTreeNode() 
	{
		Key = Enumerable.Empty<TKey>();
	}
    public RadixTreeNode(IEnumerable<TKey> key)
    {
        Key = key;
    }
    public RadixTreeNode(IEnumerable<TKey> key, RadixTreeNodeValue<TValue> value)
    {
        Key = key;
        Value = value;
        _isValueNode = true;
    }
    public RadixTreeNode(IEnumerable<TKey> key, ReadOnlyDictionary<TKey, RadixTreeNode<TKey, TValue>> children, bool isValueNode, RadixTreeNodeValue<TValue>? value)
    {
        Key = key;
		Children = children;
        _isValueNode = isValueNode;
        Value = value;
    }
}
