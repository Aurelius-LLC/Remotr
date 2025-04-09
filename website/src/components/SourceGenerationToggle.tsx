import React, { useEffect, useState } from 'react';
import styles from './SourceGenerationToggle.module.css';
import { useSourceGeneration } from './SourceGenerationContext';

export default function SourceGenerationToggle(): React.ReactElement {
  const { isEnabled, setIsEnabled } = useSourceGeneration();
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  // Don't render anything until we're mounted to avoid hydration issues
  if (!mounted) {
    return <div style={{ display: 'none' }} />;
  }

  const handleToggle = () => {
    setIsEnabled(!isEnabled);
  };

  return (
    <button
      type="button"
      role="switch"
      aria-checked={isEnabled}
      className={`${styles.toggle} ${isEnabled ? styles.enabled : ''}`}
      onClick={handleToggle}
    >
      <span className={styles.thumb} />
    </button>
  );
} 