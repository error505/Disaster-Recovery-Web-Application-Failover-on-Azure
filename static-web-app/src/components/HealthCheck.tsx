import React, { useEffect, useState } from 'react';
import { checkHealth } from '../services/apiService';

const HealthCheck: React.FC = () => {
  const [isHealthy, setIsHealthy] = useState<boolean | null>(null);

  useEffect(() => {
    const fetchHealthStatus = async () => {
      const healthStatus = await checkHealth();
      setIsHealthy(healthStatus);
    };

    fetchHealthStatus();
  }, []);

  return (
    <div>
      <h2>Application Health Check</h2>
      {isHealthy === null && <p>Checking health...</p>}
      {isHealthy !== null && (
        <p>{isHealthy ? 'Application is healthy.' : 'Application is not healthy.'}</p>
      )}
    </div>
  );
};

export default HealthCheck;
