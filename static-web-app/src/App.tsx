import React from 'react';
import InventoryForm from './components/InventoryForm';
import InventoryList from './components/InventoryList';
import HealthCheck from './components/HealthCheck';

const App: React.FC = () => {
  return (
    <div className="App">
      <h1>Inventory Management Dashboard</h1>
      <HealthCheck />
      <InventoryForm />
      <InventoryList />
    </div>
  );
};

export default App;
