import React, { ReactNode } from 'react';
import { SourceGenerationProvider } from '@site/src/components/SourceGenerationContext';

// Create a global store for the context
declare global {
  interface Window {
    __SOURCE_GENERATION_CONTEXT__: {
      isEnabled: boolean;
      setIsEnabled: (enabled: boolean) => void;
    };
  }
}

interface RootProps {
  children: ReactNode;
}

// Default implementation for the Root component
export default function Root({ children }: RootProps) {
  return (
    <SourceGenerationProvider>
      {children}
    </SourceGenerationProvider>
  );
} 