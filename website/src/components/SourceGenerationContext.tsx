import { createContext, useContext, useState, ReactNode, useEffect } from 'react';

// Create global state that works without React
let globalIsEnabled = true;
const globalListeners: Array<() => void> = [];

// Global API that doesn't require React
export const SourceGenerationAPI = {
  isEnabled: () => globalIsEnabled,
  setEnabled: (value: boolean) => {
    if (globalIsEnabled !== value) {
      globalIsEnabled = value;
      // Notify all listeners
      globalListeners.forEach(listener => listener());
    }
  },
  subscribe: (listener: () => void) => {
    globalListeners.push(listener);
    return () => {
      const index = globalListeners.indexOf(listener);
      if (index !== -1) {
        globalListeners.splice(index, 1);
      }
    };
  }
};

// Define the React context type
interface SourceGenerationContextType {
  isEnabled: boolean;
  setIsEnabled: (enabled: boolean) => void;
}

// Create the React context
const SourceGenerationContext = createContext<SourceGenerationContextType | undefined>(undefined);

// React hook to use the context
export function useSourceGeneration() {
  const context = useContext(SourceGenerationContext);
  if (context === undefined) {
    // Create a synthetic context using the global API
    throw new Error('useSourceGeneration must be used within a SourceGenerationProvider');
  }
  return context;
}

// Props for the provider component
interface SourceGenerationProviderProps {
  children: ReactNode;
  initialState?: boolean;
}

// Provider component
export function SourceGenerationProvider({ 
  children, 
  initialState = true
}: SourceGenerationProviderProps) {
  // Initialize with global state or initialState
  const [isEnabled, setIsEnabledState] = useState(globalIsEnabled || initialState);
  
  // Set the global state on mount
  useEffect(() => {
    globalIsEnabled = isEnabled;
  }, []);
  
  // Sync React state to global state
  useEffect(() => {
    // When our state changes, update global state
    if (globalIsEnabled !== isEnabled) {
      globalIsEnabled = isEnabled;
      // Notify listeners (except ourselves)
      globalListeners.forEach(listener => listener());
    }
    
    // Subscribe to global state changes
    const unsubscribe = SourceGenerationAPI.subscribe(() => {
      if (isEnabled !== globalIsEnabled) {
        setIsEnabledState(globalIsEnabled);
      }
    });
    
    return unsubscribe;
  }, [isEnabled]);
  
  // Wrapper for setIsEnabled that updates both local and global state
  const setIsEnabled = (value: boolean) => {
    setIsEnabledState(value);
    SourceGenerationAPI.setEnabled(value);
  };

  return (
    <SourceGenerationContext.Provider value={{ isEnabled, setIsEnabled }}>
      {children}
    </SourceGenerationContext.Provider>
  );
} 