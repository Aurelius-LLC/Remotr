import React, { ReactNode, useState, useEffect } from 'react';
import { useSourceGeneration, SourceGenerationAPI } from './SourceGenerationContext';

// Fallback to using the global API if outside React context
const useGenerationState = () => {
  // First try to use the React context
  try {
    return useSourceGeneration();
  } catch (e) {
    // If that fails, use the global API with a local React state
    const [isEnabled, setLocalIsEnabled] = useState(SourceGenerationAPI.isEnabled());
    
    useEffect(() => {
      // Subscribe to global changes
      const unsubscribe = SourceGenerationAPI.subscribe(() => {
        setLocalIsEnabled(SourceGenerationAPI.isEnabled());
      });
      
      return unsubscribe;
    }, []);
    
    return {
      isEnabled,
      setIsEnabled: (value: boolean) => {
        setLocalIsEnabled(value);
        SourceGenerationAPI.setEnabled(value);
      }
    };
  }
};

interface GenerationComponentProps {
  children: ReactNode;
}

export function WithGeneration({ children }: GenerationComponentProps) {
  const { isEnabled } = useGenerationState();
  if (!isEnabled) return null;
  return <>{children}</>;
}

export function WithoutGeneration({ children }: GenerationComponentProps) {
  const { isEnabled } = useGenerationState();
  if (isEnabled) return null;
  return <>{children}</>;
}

interface ToggledGenerationExampleProps {
  children: ReactNode;
}

export default function ToggledGenerationExample({ children }: ToggledGenerationExampleProps) {
  return (
    <div>
      {children}
    </div>
  );
} 