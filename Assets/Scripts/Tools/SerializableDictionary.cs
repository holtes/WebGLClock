using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Tools
{
	[Serializable]
	public struct SerializableKeyValuePair<KeysType, ValuesType>
	{
		[field: SerializeField]
		public KeysType key { get; set; }

		[field: SerializeField]
		public ValuesType value { get; set; }

		public override string ToString() =>
			$"Key: {key}; Value: {value}";

		public static SerializableKeyValuePair<KeysType, ValuesType> FromUsualPair(KeyValuePair<KeysType, ValuesType> pair) =>
			new SerializableKeyValuePair<KeysType, ValuesType>
			{
				key = pair.Key,
				value = pair.Value
			};

		public static implicit operator KeyValuePair<KeysType, ValuesType>(SerializableKeyValuePair<KeysType, ValuesType> pair) =>
			new KeyValuePair<KeysType, ValuesType>(
				key: pair.key,
				value: pair.value);
	}

	[Serializable]
	public class SerializableDictionary<KeysType, ValuesType> :
		IDictionary<KeysType, ValuesType>,
		IReadOnlyDictionary<KeysType, ValuesType>,
		IDictionary,
		ICollection,
		IEnumerable,

		IDeserializationCallback,
		ISerializable,

		ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<SerializableKeyValuePair<KeysType, ValuesType>> _keyValuePairs =
			new List<SerializableKeyValuePair<KeysType, ValuesType>>();

		private Dictionary<KeysType, ValuesType> _dictonary =
			new Dictionary<KeysType, ValuesType>();



		public ValuesType this[KeysType key]
		{
			get => _dictonary[key];
			set
			{
				_dictonary[key] = value;

				var pair =
					new SerializableKeyValuePair<KeysType, ValuesType>
					{
						key = key,
						value = value
					};
				if (!TryGetIndexOfPairByKey(key, out int pairIndex))
					_keyValuePairs.Add(
						pair);
				else
					_keyValuePairs[pairIndex] = pair;
			}
		}

		public Dictionary<KeysType, ValuesType>.KeyCollection keys =>
			_dictonary.Keys;

		public Dictionary<KeysType, ValuesType>.ValueCollection values =>
			_dictonary.Values;

		public int Count => _dictonary.Count;

		public void Add(KeysType key, ValuesType value)
		{
			_dictonary.Add(
				key,
				value);

			_keyValuePairs.Add(
				new SerializableKeyValuePair<KeysType, ValuesType>
				{
					key = key,
					value = value
				});
		}

		[ContextMenu("Clear")]
		public void Clear()
		{
			_dictonary.Clear();
			_keyValuePairs.Clear();
		}

		public bool ContainsKey(KeysType key) =>
			_dictonary.ContainsKey(
				key);

		public bool Remove(KeysType key)
		{
			var result =
				_dictonary.Remove(
			key);

			if (TryGetIndexOfPairByKey(key, out int pairIndex))
				_keyValuePairs.RemoveAt(
					pairIndex);

			return result;
		}

		public bool TryGetValue(KeysType key, out ValuesType value)
		{
			value = default;

			return
				key != null ?
					_dictonary.TryGetValue(
						key,
						out value) :
					false;
		}

		public IEnumerator<KeyValuePair<KeysType, ValuesType>> GetEnumerator() =>
			_dictonary.GetEnumerator();

		bool TryGetIndexOfPairByKey(KeysType key, out int index)
		{
			index = 0;

			for (int p = 0; p < _keyValuePairs.Count; p++)
				if (_keyValuePairs[p].key != null && key != null &&
					_keyValuePairs[p].key.GetHashCode() == key.GetHashCode())
				{
					index = p;
					return true;
				}

			return false;
		}

		bool ValidatePairs(List<SerializableKeyValuePair<KeysType, ValuesType>> pairs)
		{
			_dictonary.Clear();

			var indexesOfPairsBeingDeleted =
				new List<int>(_keyValuePairs.Count);

			SerializableKeyValuePair<KeysType, ValuesType>? pairWithUndefinedKey = null;

			var result = true;
			for (int p = 0; p < _keyValuePairs.Count; p++)
				if (_keyValuePairs[p].key == null)
				{
					if (!pairWithUndefinedKey.HasValue)
						pairWithUndefinedKey = _keyValuePairs[p];

					indexesOfPairsBeingDeleted.Add(
						p);

					result = false;
				}
				else
				if (_dictonary.ContainsKey(_keyValuePairs[p].key))
				{
					if (!pairWithUndefinedKey.HasValue)
						pairWithUndefinedKey =
							new SerializableKeyValuePair<KeysType, ValuesType>
							{
								value = _keyValuePairs[p].value
							};

					indexesOfPairsBeingDeleted.Add(
						p);

					result = false;
				}
				else
					_dictonary.Add(
						_keyValuePairs[p].key,
						_keyValuePairs[p].value);

			for (int i = indexesOfPairsBeingDeleted.Count - 1; i >= 0; i--)
				_keyValuePairs.RemoveAt(
					indexesOfPairsBeingDeleted[i]);

			if (pairWithUndefinedKey.HasValue)
				_keyValuePairs.Add(
					pairWithUndefinedKey.Value);

			return result;
		}

		#region IDictionary<KeysType, ValuesType>
		ICollection<KeysType> IDictionary<KeysType, ValuesType>.Keys =>
			keys;
		ICollection<ValuesType> IDictionary<KeysType, ValuesType>.Values =>
			values;
		#endregion

		#region IReadOnlyDictionary<KeysType, ValuesType>
		IEnumerable<KeysType> IReadOnlyDictionary<KeysType, ValuesType>.Keys =>
			keys;
		IEnumerable<ValuesType> IReadOnlyDictionary<KeysType, ValuesType>.Values =>
			values;
		#endregion

		#region ICollection<KeyValuePair<KeysType, ValuesType>>
		bool ICollection<KeyValuePair<KeysType, ValuesType>>.IsReadOnly => false;
		bool ICollection<KeyValuePair<KeysType, ValuesType>>.Contains(KeyValuePair<KeysType, ValuesType> item) =>
			_dictonary.TryGetValue(
				item.Key,
				out ValuesType value) &&
			value.Equals(item.Value);
		void ICollection<KeyValuePair<KeysType, ValuesType>>.Add(KeyValuePair<KeysType, ValuesType> item) =>
			Add(
				item.Key,
				item.Value);
		bool ICollection<KeyValuePair<KeysType, ValuesType>>.Remove(KeyValuePair<KeysType, ValuesType> item) =>
			Remove(
				item.Key);
		void ICollection<KeyValuePair<KeysType, ValuesType>>.CopyTo(KeyValuePair<KeysType, ValuesType>[] array, int arrayIndex) =>
			(_dictonary as ICollection<KeyValuePair<KeysType, ValuesType>>).
				CopyTo(
					array,
					arrayIndex);
		#endregion

		#region IDictionary
		bool IDictionary.IsFixedSize =>
			false;
		bool IDictionary.IsReadOnly =>
			false;
		ICollection IDictionary.Keys =>
			keys;
		ICollection IDictionary.Values =>
			values;
		bool ICollection.IsSynchronized =>
			(_dictonary as ICollection).IsSynchronized;
		object ICollection.SyncRoot =>
			(_dictonary as ICollection).SyncRoot;
		object IDictionary.this[object key]
		{
			get => _dictonary[(KeysType)key];
			set => this[(KeysType)key] = (ValuesType)value;
		}
		void IDictionary.Add(object key, object value) =>
			Add(
				(KeysType)key,
				(ValuesType)value);
		bool IDictionary.Contains(object key) =>
			ContainsKey(
				(KeysType)key);
		IDictionaryEnumerator IDictionary.GetEnumerator() =>
			(_dictonary as IDictionary).GetEnumerator();
		void IDictionary.Remove(object key) =>
			Remove(
				(KeysType)key);
		void ICollection.CopyTo(Array array, int index) =>
			(_dictonary as ICollection).CopyTo(
				array,
				index);
		#endregion

		#region IEnumerable<KeyValuePair<KeysType, ValuesType>>
		IEnumerator<KeyValuePair<KeysType, ValuesType>> IEnumerable<KeyValuePair<KeysType, ValuesType>>.GetEnumerator() =>
			_dictonary.GetEnumerator();
		#endregion

		#region IEnumerable
		IEnumerator IEnumerable.GetEnumerator() =>
			_dictonary.GetEnumerator();
		#endregion

		#region IDeserializationCallback
		void IDeserializationCallback.OnDeserialization(object sender) =>
			(_dictonary as IDeserializationCallback).OnDeserialization(
				sender);
		#endregion

		#region ISerializable
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) =>
			(_dictonary as ISerializable).GetObjectData(
				info,
				context);
		#endregion

		#region ISerializationCallbackReceiver
		void ISerializationCallbackReceiver.OnBeforeSerialize() =>
			ValidatePairs(
				_keyValuePairs);
		void ISerializationCallbackReceiver.OnAfterDeserialize() =>
			ValidatePairs(
				_keyValuePairs);
		#endregion
	}
}