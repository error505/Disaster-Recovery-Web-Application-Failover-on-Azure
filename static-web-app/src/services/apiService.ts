import axios from 'axios';

// Base URL for the API endpoints
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'https://your-api.azurewebsites.net/api';

export interface InventoryItem {
  id: string;
  name: string;
  quantity: number;
  location: string;
}

// Function to add a new inventory item
export const addInventoryItem = async (item: InventoryItem): Promise<void> => {
  await axios.post(`${API_BASE_URL}/inventory/add-item`, item);
};

// Function to get an inventory item by ID
export const getInventoryItem = async (id: string): Promise<InventoryItem> => {
  const response = await axios.get(`${API_BASE_URL}/inventory/get-item/${id}`);
  return response.data;
};

// Function to check the health of the API
export const checkHealth = async (): Promise<boolean> => {
  try {
    const response = await axios.get(`${API_BASE_URL}/health`);
    return response.status === 200;
  } catch (error) {
    return false;
  }
};
