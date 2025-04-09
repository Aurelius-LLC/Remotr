import ExecutionEnvironment from '@docusaurus/ExecutionEnvironment';
import { SourceGenerationAPI } from '../components/SourceGenerationContext';
import styles from '../components/SourceGenerationToggle.module.css';

// Execute only in the browser
if (ExecutionEnvironment.canUseDOM) {
  // Create a toggle element without using React
  const createToggle = () => {
    const toggle = document.createElement('button');
    toggle.setAttribute('type', 'button');
    toggle.setAttribute('role', 'switch');
    toggle.setAttribute('aria-checked', String(SourceGenerationAPI.isEnabled()));
    toggle.className = styles.toggle + (SourceGenerationAPI.isEnabled() ? ' ' + styles.enabled : '');
    
    const thumb = document.createElement('span');
    thumb.className = styles.thumb;
    toggle.appendChild(thumb);
    
    // Add click handler
    toggle.addEventListener('click', () => {
      const newState = !SourceGenerationAPI.isEnabled();
      SourceGenerationAPI.setEnabled(newState);
      toggle.setAttribute('aria-checked', String(newState));
      if (newState) {
        toggle.classList.add(styles.enabled);
      } else {
        toggle.classList.remove(styles.enabled);
      }
    });
    
    // Subscribe to global state changes
    SourceGenerationAPI.subscribe(() => {
      const isEnabled = SourceGenerationAPI.isEnabled();
      toggle.setAttribute('aria-checked', String(isEnabled));
      if (isEnabled) {
        toggle.classList.add(styles.enabled);
      } else {
        toggle.classList.remove(styles.enabled);
      }
    });
    
    return toggle;
  };

  // Try to find the sidebar menu and inject the toggle
  const createToggleContainer = () => {
    // Find the sidebar nav or the docs sidebar (try both selectors)
    const sidebarNav = document.querySelector('.menu__list') || 
                       document.querySelector('.theme-doc-sidebar-menu');
    
    // If toggle already exists, don't create another
    if (document.getElementById('source-generation-toggle-container')) {
      return;
    }
    
    if (!sidebarNav) {
      // If we can't find it yet, try again in 100ms
      console.log('Sidebar not found, retrying...');
      setTimeout(createToggleContainer, 300);
      return;
    }
    
    console.log('Sidebar found, creating toggle container');
    
    // Create container for our toggle
    const toggleContainer = document.createElement('div');
    toggleContainer.id = 'source-generation-toggle-container';
    toggleContainer.className = styles.toggleContainer;
    
    // Create label for the toggle
    const labelElement = document.createElement('span');
    labelElement.textContent = 'Use Source Generation';
    labelElement.className = styles.label;
    
    // Add the label to the container
    toggleContainer.appendChild(labelElement);
    
    // Create the toggle and add it to the container
    const toggle = createToggle();
    toggleContainer.appendChild(toggle);
    
    // Insert at top of sidebar
    sidebarNav.parentNode?.insertBefore(toggleContainer, sidebarNav);
    
    console.log('Toggle container successfully created and inserted');
    
    // Also apply the initial state to the page's body for CSS hooks
    if (SourceGenerationAPI.isEnabled()) {
      document.body.classList.add('source-generation-enabled');
    } else {
      document.body.classList.remove('source-generation-enabled');
    }
    
    // Subscribe to state changes for CSS classes
    SourceGenerationAPI.subscribe(() => {
      if (SourceGenerationAPI.isEnabled()) {
        document.body.classList.add('source-generation-enabled');
      } else {
        document.body.classList.remove('source-generation-enabled');
      }
    });
  };
  
  // Try to create the toggle when document is ready
  const initializeToggle = () => {
    console.log('Initializing source generation toggle');
    createToggleContainer();
    
    // Also observe DOM changes to catch sidebar that loads after initial page load
    const observer = new MutationObserver((mutations, obs) => {
      // If we find the sidebar and haven't created the toggle yet, create it
      if ((document.querySelector('.menu__list') || document.querySelector('.theme-doc-sidebar-menu')) && 
          !document.getElementById('source-generation-toggle-container')) {
        console.log('Sidebar detected via MutationObserver');
        createToggleContainer();
      }
    });
    
    observer.observe(document.body, { 
      childList: true,
      subtree: true
    });
  };
  
  // Start looking for the sidebar when the document is loaded
  if (document.readyState === 'loading') {
    window.addEventListener('DOMContentLoaded', initializeToggle);
  } else {
    initializeToggle();
  }
  
  // Also try on route changes for SPA navigation
  window.addEventListener('popstate', createToggleContainer);
} 