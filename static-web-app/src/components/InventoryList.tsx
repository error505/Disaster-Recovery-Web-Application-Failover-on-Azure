import React, { useState } from 'react';
import { getInventoryItem, InventoryItem } from '../services/apiService';

const InventoryList: React.FC = () => {
  const [itemId, setItemId] = useState('');
  const [item, setItem] = useState<InventoryItem | null>(null);
  const [message, setMessage] = useState('');

  const handleSearch = async () => {
    try {
      const fetchedItem = await getInventoryItem(itemId);
      setItem(fetchedItem);
      setMessage('');
    } catch (error) {
      setMessage('Item not found. Please try again.');
      setItem(null);
    }
  };

  return (
    <div>
      <h2>Search Inventory Item</h2>
      <input
        type="text"
        value={itemId}
        onChange={(e) => setItemId(e.target.value)}
        placeholder="Item ID"
      />
      <button onClick={handleSearch}>Search</button>
      {message && <p>{message}</p>}
      {item && (
        <div>
          <p><strong>ID:</strong> {item.id}</p>
          <p><strong>Name:</strong> {item.name}</p>
          <p><strong>Quantity:</strong> {item.quantity}</p>
          <p><strong>Location:</strong> {item.location}</p>
        </div>
      )}
    </div>
  );
};

export default InventoryList;
