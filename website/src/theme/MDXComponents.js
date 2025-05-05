import React from 'react';
// Import the original MDX components
import MDXComponents from '@theme-original/MDXComponents';
// Import our toggle generation components
import ToggledGenerationExample, { 
  WithGeneration, 
  WithoutGeneration 
} from '@site/src/components/ToggledGenerationExample';
// Import Docusaurus tab components
import Tabs from '@theme/Tabs';
import TabItem from '@theme/TabItem';

export default {
  // Re-use the default MDX components
  ...MDXComponents,
  // Add our custom components
  ToggledGenerationExample,
  WithGeneration,
  WithoutGeneration,
  // Add tab components
  Tabs,
  TabItem
}; 