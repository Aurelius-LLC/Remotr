import React, { useRef, CSSProperties, useEffect } from 'react';
import { transitionsCount, useTransitionManager } from './transitions/TransitionManager';

const stepMetadata = [
  {
    fragment: "aggregates-defined",
    title: "What are Aggregates in Remotr?",
    description: <p>
      Aggregates in Remotr are loosely coupled groups of stateful entities that...<br /><br />
      <ul>
        <li>
          <a target="_blank" href="https://www.etteplan.com/about-us/insights/how-virtual-actors-will-help-you-scale-your-applications-easy-way/">follow the virtual actor model</a>
        </li>
        <li>
        <a target="_blank" href="https://www.geeksforgeeks.org/cqrs-command-query-responsibility-segregation/">use CQRS</a>
        </li>
        <li>
          have transactional state across the whole aggregate
        </li>
      </ul>
    </p>,
  },
  {
    fragment: "calling-aggregates",
    title: "How to call or interface with aggregates?",
    description: <p>Communication to aggregates goes through a singleton "AggregateRoot" which describes how the aggregates entities can be accessed. Usually, the entire aggregate revolves around this entity.</p>,
  },
  {
    fragment: "cqrs-aggregates",
    title: "Command Query Responsibility Segregation (CQRS)",
    description: <p>You can't call methods on Aggregates. Instead, you call commands or queries on the AggregateRoot. Commands allow for state changes, but queries guarantee that the entire state of the aggregate, including all its entities, will remain unchanged.</p>,
  },
  
  {
    fragment: "cqrs-entities",
    title: "CQRS pattern in Entities",
    description: <p>Entities operate with the same concept in terms of CQRS; however, entities can only be accessed by their AggregateRoot. For example, RootA cannot call the entity of RootB and vice versa.<br /><br />Entities can also call commands/queries of other entities, allowing for rich and dynamic composition of state.</p>,
  },
  {
    fragment: "cqrs-infectious",
    title: 'Queries are "infectious"',
    description: <p>Similar to `async`, queries in the Remotr CQRS API are "infectious". Anything declared as a query can only call other queries, all the way down; however, commands can call other commands or queries.<br /><br />This gives assurance that all queries will never result in a state change anywhere.</p>,
  },
  {
    // TODO: Insert link for deadlocks page.
    // TODO: Insert link for sagas eventually.
    // TODO: Insert link at bottom for next page.
    fragment: "root-to-root",
    title: 'Root to Root calls',
    description: <p>The commands or queries of an AggregateRoot can call queries on other AggregateRoots, but not commands. <br /><br />This is because different aggregates could be placed on different nodes in the cluster, and distributed transactions aren't supported with Remotr.  <br /><br />This also drastically reduces the risk of deadlocking as it's nearly impossible to create a deadlock within an aggregate, and queries can't deadlock by nature.</p>,
  }
];

export default function OverviewAnimation(): React.JSX.Element {
  const svgRef = useRef<SVGSVGElement>(null);
  const { currentStep, isAnimating, handleNext, handleBack, skipToStep } = useTransitionManager(svgRef as React.RefObject<SVGSVGElement>);
  const totalSteps = transitionsCount + 1;
  const isUrlNavigationRef = useRef(false);
  const hasInitializedRef = useRef(false);

  // Handle initial navigation from URL fragment
  useEffect(() => {
    if (!hasInitializedRef.current && window.location.hash) {
      const hash = window.location.hash.slice(1);
      const stepIndex = stepMetadata.findIndex(step => step.fragment === hash);
      if (stepIndex !== -1) {
        isUrlNavigationRef.current = true;
        skipToStep(stepIndex + 1);
      }
    }
    hasInitializedRef.current = true;
  }, []); // Run only once on mount

  // Update URL fragment when step changes
  useEffect(() => {
    const fragment = stepMetadata[currentStep - 1].fragment;
    if (window.location.hash.slice(1) !== fragment && !isUrlNavigationRef.current) {
      window.location.hash = fragment;
    }
    isUrlNavigationRef.current = false;
  }, [currentStep]);

  // Handle URL fragment changes after initial load
  useEffect(() => {
    const handleHashChange = () => {
      const hash = window.location.hash.slice(1);
      const stepIndex = stepMetadata.findIndex(step => step.fragment === hash);
      if (stepIndex !== -1 && stepIndex + 1 !== currentStep) {
        isUrlNavigationRef.current = true;
        skipToStep(stepIndex + 1);
      }
    };

    // Set initial hash if none present
    if (!window.location.hash) {
      window.location.hash = stepMetadata[currentStep - 1].fragment;
    }

    // Listen for hash changes
    window.addEventListener('hashchange', handleHashChange);
    return () => window.removeEventListener('hashchange', handleHashChange);
  }, [skipToStep, currentStep]);

  // Button styles
  const buttonStyle = {
    padding: '10px 20px',
    margin: '0 10px',
    backgroundColor: '#1FA2FF',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    fontSize: '16px',
  };

  const disabledButtonStyle = {
    ...buttonStyle,
    backgroundColor: '#888',
    cursor: 'not-allowed',
  };

  // Dropdown styles
  const dropdownStyle: CSSProperties = {
    padding: '12px 16px',
    fontSize: '16px',
    backgroundColor: '#555',
    color: 'white',
    borderRadius: '8px',
    cursor: 'pointer',
    width: '100%',
    marginBottom: '20px',
    appearance: 'none' as const,
    backgroundImage: `url("data:image/svg+xml;charset=UTF-8,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='white'%3e%3cpath d='M7 10l5 5 5-5z'/%3e%3c/svg%3e")`,
    backgroundRepeat: 'no-repeat',
    backgroundPosition: 'right 12px center',
    backgroundSize: '20px',
    textAlign: 'left'
  };

  const dropdownContainerStyle = {
    display: 'block',
    position: 'relative' as const
  };

  const handleStepChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    skipToStep(Number(event.target.value));
  };

  
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '20px', width: '100%' }}>
      <div style={{ flex: '0 0 60%' }}>
        <svg 
          ref={svgRef} 
          width="100%" 
          height="100%" 
          viewBox="0 0 800 680" 
          xmlns="http://www.w3.org/2000/svg"
          preserveAspectRatio="xMidYMid meet"
        >
          {/* Define colors for our bubbles */}
          <defs>
            {/* Light pastel colors for the aggregate bubbles */}
            <linearGradient id="customerGradient" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stopColor="#a8d0e6" />
              <stop offset="100%" stopColor="#8fb3d9" />
            </linearGradient>
            <linearGradient id="storeGradient" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stopColor="#f8a978" />
              <stop offset="100%" stopColor="#f08c5a" />
            </linearGradient>
            <linearGradient id="orderGradient" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stopColor="#c8b6ff" />
              <stop offset="100%" stopColor="#b39ddb" />
            </linearGradient>
            
            {/* Colors for the root and entity bubbles */}
            <linearGradient id="rootGradient" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stopColor="#59405c" />
              <stop offset="100%" stopColor="#4a3a4c" />
            </linearGradient>
          </defs>

          <g style={{ transform: 'translateX(0px) translateY(35px)' }}>
            {/* Customer Aggregate - positioned at the top of the triangle */}
            <g data-aggregate="customer" style={{ transform: 'translateX(400px) translateY(180px)' }}>
              {/* Main aggregate bubble */}
              <circle cx="0" cy="0" r="150" fill="url(#customerGradient)" opacity="0.8"/>
              <text x="0" y="-170" fontFamily="Arial" fontSize="18" textAnchor="middle" fontWeight="bold" fill="var(--ifm-color-emphasis-900)">Customer Aggregate</text>
              
              {/* Root bubble - positioned at the top inside the aggregate */}
              <circle cx="0" cy="-70" r="50" fill="#2E5A35"/>
              <text x="0" y="-70" fontFamily="Arial" fontSize="14" textAnchor="middle" fill="white">CustomerRoot</text>
              <text x="0" y="-55" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">(root)</text>
              
              {/* Entity bubbles - positioned around the root with more space */}
              <circle cx="-80" cy="20" r="40" fill="black"/>
              <text x="-80" y="15" fontFamily="Arial" fontSize="11" textAnchor="middle" fill="white">ProfileEntity</text>
              <text x="-80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              
              <circle cx="0" cy="70" r="40" fill="black"/>
              <text x="0" y="65" fontFamily="Arial" fontSize="11" textAnchor="middle" fill="white">HistoryEntity</text>
              <text x="0" y="80" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              
              <circle cx="80" cy="20" r="40" fill="black"/>
              <text x="80" y="15" fontFamily="Arial" fontSize="11" textAnchor="middle" fill="white">WishListEntity</text>
              <text x="80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
            </g>
            
            <g data-commands="customer" style={{ transform: 'translateX(320px) translateY(140px) scale(1.9)' }} opacity="0">
              <rect x="131" width="88" height="24" rx="7" fill="#FF4D4D"/>
              <text x="155.004" y="19.6818" fill="white" fontFamily="Arial" fontSize="6">Root Command</text>
              <text x="144.143" y="10.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">CreateUserCommand</text>
              
              <rect x="131" y="26" width="88" height="24" rx="7" fill="#1FA2FF"/>
              <text x="160.39" y="45.6818" fill="white" fontFamily="Arial" fontSize="6">Root Query</text>
              <text x="155.324" y="36.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">GetUserQuery</text>
              
              <rect x="131" y="78" width="88" height="24" rx="7" fill="#1FA2FF"/>
              <text x="160.39" y="97.6818" fill="white" fontFamily="Arial" fontSize="6">Root Query</text>
              <text x="155.246" y="88.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">GetCartQuery</text>
              
              <rect x="131" y="52" width="88" height="24" rx="7" fill="#FF4D4D"/>
              <text x="155.004" y="71.6818" fill="white" fontFamily="Arial" fontSize="6">Root Command</text>
              <text x="136.162" y="62.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">ProcessPaymentCommand</text>
              
              <path className="arrow" d="M9.18627 32.4188C8.86526 32.8682 8.96935 33.4927 9.41876 33.8137L16.7424 39.0449C17.1918 39.3659 17.8163 39.2618 18.1373 38.8124C18.4583 38.363 18.3543 37.7384 17.9048 37.4174L11.395 32.7675L16.0449 26.2576C16.3659 25.8082 16.2618 25.1837 15.8124 24.8627C15.363 24.5417 14.7384 24.6457 14.4174 25.0952L9.18627 32.4188ZM123.836 13.0136L9.8356 32.0136L10.1644 33.9864L124.164 14.9864L123.836 13.0136Z"/>
              <path className="arrow" d="M0.264971 43.322C-0.109502 43.7279 -0.0839922 44.3605 0.321949 44.735L6.93715 50.8374C7.34309 51.2119 7.97574 51.1864 8.35021 50.7804C8.72468 50.3745 8.69917 49.7418 8.29323 49.3674L2.41306 43.943L7.83741 38.0628C8.21188 37.6569 8.18637 37.0243 7.78043 36.6498C7.37449 36.2753 6.74183 36.3008 6.36736 36.7068L0.264971 43.322ZM124.96 38.0008L0.959703 43.0008L1.04028 44.9992L125.04 39.9992L124.96 38.0008Z"/>
              <path className="arrow" d="M0.316252 59.2703C-0.0867593 59.6479 -0.107339 60.2807 0.270286 60.6838L6.42404 67.2512C6.80166 67.6542 7.43449 67.6748 7.8375 67.2972C8.24051 66.9195 8.26109 66.2867 7.88347 65.8837L2.41347 60.046L8.2512 54.576C8.65421 54.1983 8.67479 53.5655 8.29717 53.1625C7.91954 52.7595 7.28671 52.7389 6.8837 53.1165L0.316252 59.2703ZM124.033 63.0005L1.03251 59.0005L0.967499 60.9995L123.967 64.9995L124.033 63.0005Z"/>
              <path className="arrow" d="M4.39461 72.2041C3.95503 72.5384 3.86972 73.1658 4.20407 73.6054L9.65257 80.7688C9.98692 81.2083 10.6143 81.2936 11.0539 80.9593C11.4935 80.6249 11.5788 79.9976 11.2444 79.558L6.40132 73.1905L12.7688 68.3474C13.2083 68.0131 13.2936 67.3857 12.9593 66.9461C12.6249 66.5065 11.9976 66.4212 11.558 66.7556L4.39461 72.2041ZM121.986 87.8977L5.13473 72.0091L4.86527 73.9909L121.717 89.8794L121.986 87.8977Z"/>
            </g>
            
            {/* Store Aggregate - positioned at the bottom left of the triangle */}
            <g data-aggregate="store" style={{ transform: 'translateX(200px) translateY(440px)' }}>
              {/* Main aggregate bubble */}
              <circle cx="0" cy="0" r="150" fill="url(#storeGradient)" opacity="0.8"/>
              <text x="0" y="180" fontFamily="Arial" fontSize="18" textAnchor="middle" fontWeight="bold" fill="var(--ifm-color-emphasis-900)">Store Aggregate</text>
              
              {/* Root bubble - positioned at the top inside the aggregate */}
              <circle cx="0" cy="-70" r="50" fill="#2E5A35"/>
              <text x="0" y="-70" fontFamily="Arial" fontSize="14" textAnchor="middle" fill="white">StoreRoot</text>
              <text x="0" y="-55" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">(root)</text>
              
              {/* Entity bubbles - positioned around the root with more space */}
              <circle cx="-80" cy="20" r="40" fill="black"/>
              <text x="-80" y="15" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">BillingEntity</text>
              <text x="-80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              
              <circle cx="0" cy="70" r="40" fill="black"/>
              <text x="0" y="65" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">StockEntity</text>
              <text x="0" y="80" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              
              <circle cx="80" cy="20" r="45" fill="black"/>
              <text x="80" y="15" fontFamily="Arial" fontSize="10.5" textAnchor="middle" fill="white">PurchasesEntity</text>
              <text x="80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
            </g>
            
            {/* Order Aggregate - positioned at the bottom right of the triangle */}
            <g data-aggregate="order" style={{ transform: 'translateX(600px) translateY(440px)' }}>
              {/* Main aggregate bubble */}
              <circle cx="0" cy="0" r="150" fill="url(#orderGradient)" opacity="0.8"/>
              <text x="0" y="180" fontFamily="Arial" fontSize="18" textAnchor="middle" fontWeight="bold" fill="var(--ifm-color-emphasis-900)">Order Aggregate</text>
              
              {/* Root bubble - positioned at the top inside the aggregate */}
              <circle cx="0" cy="-70" r="50" fill="#2E5A35"/>
              <text x="0" y="-70" fontFamily="Arial" fontSize="14" textAnchor="middle" fill="white">OrderRoot</text>
              <text x="0" y="-55" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">(root)</text>
              
              {/* Entity bubbles - positioned around the root with more space */}
              <circle cx="-80" cy="20" r="40" fill="black"/>
              <text x="-80" y="15" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">OrderEntity</text>
              <text x="-80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              
              <circle cx="0" cy="70" r="40" fill="black"/>
              <text x="0" y="65" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">ItemEntity</text>
              <text x="0" y="80" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              
              <circle cx="80" cy="20" r="40" fill="black"/>
              <text x="80" y="15" fontFamily="Arial" fontSize="11" textAnchor="middle" fill="white">PaymentEntity</text>
              <text x="80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
            </g>

            {/* Step 4 elements */}
            <g data-step="4" style={{ transform: 'translateX(50px) translateY(200px)', opacity: 0 }}>
              <g data-step="4root" style={{ transform: 'translateX(200px) translateY(150px) scale(1.5)' }}>
                <circle cx="0" cy="-70" r="50" fill="rgba(46,90,53,1)"></circle>
                <text x="0" y="-70" fontFamily="Arial" fontSize="14" textAnchor="middle" fill="white">CustomerRoot</text>
                <text x="0" y="-55" fontFamily="Arial" fontSize="12" textAnchor="middle" fill="white">(root)</text>
              </g>

              <g data-step="4entity" style={{ transform: 'translateX(200px) translateY(350px) scale(1.5)' }}>
                <circle cx="80" cy="20" r="40" fill="black"></circle>
                <text x="80" y="15" fontFamily="Arial" fontSize="11" textAnchor="middle" fill="white">WishListEntity</text>
                <text x="80" y="30" fontFamily="Arial" fontSize="10" textAnchor="middle" fill="white">(entity)</text>
              </g>

              <g style={{ transform: 'translateX(5px) translateY(100px) scale(1.5)' }}>
                <rect width="88" height="24" rx="7" fill="#FF4D4D"/>
                <text style={{ fill: 'white', whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}>
                  <tspan x="21.0889" y="19.6818">Root Command</tspan>
                </text>
                <text style={{ fill: 'white', whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}>
                  <tspan x="13.1426" y="10.6818">CreateUserCommand</tspan>
                </text>
                <rect x="245" width="95" height="24" rx="7" fill="#FF4D4D"/>
                <text style={{ fill: 'white', whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}>
                  <tspan x="267.051" y="18.6818">Entity Command</tspan>
                </text>
                <text style={{ fill: 'white', whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}>
                  <tspan x="252.032" y="9.68182">CreateUserEntityCommand</tspan>
                </text>
                <path className="arrow" d="M114.707 12.7071C115.098 12.3166 115.098 11.6834 114.707 11.2929L108.343 4.92893C107.953 4.53841 107.319 4.53841 106.929 4.92893C106.538 5.31946 106.538 5.95262 106.929 6.34315L112.586 12L106.929 17.6569C106.538 18.0474 106.538 18.6805 106.929 19.0711C107.319 19.4616 107.953 19.4616 108.343 19.0711L114.707 12.7071ZM93 13H114V11H93V13Z" />
                <path className="arrow" d="M234.707 12.7071C235.098 12.3166 235.098 11.6834 234.707 11.2929L228.343 4.92893C227.953 4.53841 227.319 4.53841 226.929 4.92893C226.538 5.31946 226.538 5.95262 226.929 6.34315L232.586 12L226.929 17.6569C226.538 18.0474 226.538 18.6805 226.929 19.0711C227.319 19.4616 227.953 19.4616 228.343 19.0711L234.707 12.7071ZM211 13L234 13L234 11L211 11L211 13Z" />
                <path className="arrow" d="M368.707 11.7071C369.098 11.3166 369.098 10.6834 368.707 10.2929L362.343 3.92893C361.953 3.53841 361.319 3.53841 360.929 3.92893C360.538 4.31946 360.538 4.95262 360.929 5.34315L366.586 11L360.929 16.6569C360.538 17.0474 360.538 17.6805 360.929 18.0711C361.319 18.4616 361.953 18.4616 362.343 18.0711L368.707 11.7071ZM345 12L368 12L368 10L345 10L345 12Z" />
              </g>
            </g>

            {/* Step 5 elements */}
            <g data-step="5"  style={{ opacity: 0, transform: 'translateX(60px) translateY(185px) scale(1.4)' }}>
              <rect x="248" y="18" width="88" height="24" rx="7" fill="#1FA2FF"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="273.437" y="37.6818">Entity Query</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="280.136" y="28.6818">FindItem</tspan></text>
              <rect y="31.5" width="88" height="24" rx="7" fill="#FF4D4D"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="21.0889" y="51.1818">Root Command</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="18.2227" y="42.1818">AddItemToWishes</tspan></text>
              <path className="arrow" d="M114.707 44.2071C115.098 43.8166 115.098 43.1834 114.707 42.7929L108.343 36.4289C107.953 36.0384 107.319 36.0384 106.929 36.4289C106.538 36.8195 106.538 37.4526 106.929 37.8431L112.586 43.5L106.929 49.1569C106.538 49.5474 106.538 50.1805 106.929 50.5711C107.319 50.9616 107.953 50.9616 108.343 50.5711L114.707 44.2071ZM93 44.5H114V42.5H93V44.5Z" />
              <rect x="245" y="49" width="95" height="24" rx="7" fill="#FF4D4D"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="267.051" y="67.6818">Entity Command</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="279.499" y="58.6818">AddItem</tspan></text>
              <path className="arrow" d="M234.707 61.7071C235.098 61.3166 235.098 60.6834 234.707 60.2929L228.343 53.9289C227.953 53.5384 227.319 53.5384 226.929 53.9289C226.538 54.3195 226.538 54.9526 226.929 55.3431L232.586 61L226.929 66.6569C226.538 67.0474 226.538 67.6805 226.929 68.0711C227.319 68.4616 227.953 68.4616 228.343 68.0711L234.707 61.7071ZM211 62L234 62L234 60L211 60L211 62Z" />
              <path className="arrow" d="M234.707 31.7071C235.098 31.3166 235.098 30.6834 234.707 30.2929L228.343 23.9289C227.953 23.5384 227.319 23.5384 226.929 23.9289C226.538 24.3195 226.538 24.9526 226.929 25.3431L232.586 31L226.929 36.6569C226.538 37.0474 226.538 37.6805 226.929 38.0711C227.319 38.4616 227.953 38.4616 228.343 38.0711L234.707 31.7071ZM211 32L234 32L234 30L211 30L211 32Z" />
              <path className="arrow" d="M368.707 60.7071C369.098 60.3166 369.098 59.6834 368.707 59.2929L362.343 52.9289C361.953 52.5384 361.319 52.5384 360.929 52.9289C360.538 53.3195 360.538 53.9526 360.929 54.3431L366.586 60L360.929 65.6569C360.538 66.0474 360.538 66.6805 360.929 67.0711C361.319 67.4616 361.953 67.4616 362.343 67.0711L368.707 60.7071ZM345 61L368 61L368 59L345 59L345 61Z" />
              <path className="arrow" d="M368.707 30.7071C369.098 30.3166 369.098 29.6834 368.707 29.2929L362.343 22.9289C361.953 22.5384 361.319 22.5384 360.929 22.9289C360.538 23.3195 360.538 23.9526 360.929 24.3431L366.586 30L360.929 35.6569C360.538 36.0474 360.538 36.6805 360.929 37.0711C361.319 37.4616 361.953 37.4616 362.343 37.0711L368.707 30.7071ZM345 31L368 31L368 29L345 29L345 31Z" />
              <path d="M161 86C184.748 86 204 66.7482 204 43C204 19.2518 184.748 0 161 0C137.252 0 118 19.2518 118 43C118 66.7482 137.252 86 161 86Z" fill="#2E5A35"></path>
              <text fill="white" style={{ whiteSpace: "pre", fontFamily: "Arial", fontSize: "12.04px", letterSpacing: "0em" }}><tspan x="119.791" y="42.8382">CustomerRoot</tspan></text>
              <text fill="white" style={{ whiteSpace: "pre", fontFamily: "Arial", fontSize: "10.32px", letterSpacing: "0em" }}><tspan x="147.635" y="55.3328">(root)</tspan></text>
              <path d="M408 77.5C426.778 77.5 442 62.2777 442 43.5C442 24.7223 426.778 9.5 408 9.5C389.222 9.5 374 24.7223 374 43.5C374 62.2777 389.222 77.5 408 77.5Z" fill="black"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '9.35px', letterSpacing: '0em' }}><tspan x="376.727" y="42.379">WishListEntity</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '8.5px', letterSpacing: '0em' }}><tspan x="393.173" y="55.1699">(entity)</tspan></text>
              <rect x="248" y="126" width="88" height="24" rx="7" fill="#1FA2FF"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="273.437" y="145.682">Entity Query</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="280.136" y="136.682">FindItem</tspan></text>
              <rect y="139.5" width="88" height="24" rx="7" fill="#1FA2FF"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="27.4746" y="159.182">Root Query</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="32.1357" y="150.182">FindItem</tspan></text>
              <path className="arrow" d="M114.707 152.207C115.098 151.817 115.098 151.183 114.707 150.793L108.343 144.429C107.953 144.038 107.319 144.038 106.929 144.429C106.538 144.819 106.538 145.453 106.929 145.843L112.586 151.5L106.929 157.157C106.538 157.547 106.538 158.181 106.929 158.571C107.319 158.962 107.953 158.962 108.343 158.571L114.707 152.207ZM93 152.5H114V150.5H93V152.5Z" />
              <rect x="245" y="157" width="95" height="24" rx="7" fill="#FF4D4D"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="267.051" y="175.682">Entity Command</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="279.499" y="166.682">AddItem</tspan></text>
              <path className="arrow" d="M234.707 169.707C235.098 169.317 235.098 168.683 234.707 168.293L228.343 161.929C227.953 161.538 227.319 161.538 226.929 161.929C226.538 162.319 226.538 162.953 226.929 163.343L232.586 169L226.929 174.657C226.538 175.047 226.538 175.681 226.929 176.071C227.319 176.462 227.953 176.462 228.343 176.071L234.707 169.707ZM211 170L234 170L234 168L211 168L211 170Z" fill="black"/>
              <path className="arrow" d="M234.707 139.707C235.098 139.317 235.098 138.683 234.707 138.293L228.343 131.929C227.953 131.538 227.319 131.538 226.929 131.929C226.538 132.319 226.538 132.953 226.929 133.343L232.586 139L226.929 144.657C226.538 145.047 226.538 145.681 226.929 146.071C227.319 146.462 227.953 146.462 228.343 146.071L234.707 139.707ZM211 140L234 140L234 138L211 138L211 140Z" fill="black"/>
              <path className="arrow" d="M368.707 168.707C369.098 168.317 369.098 167.683 368.707 167.293L362.343 160.929C361.953 160.538 361.319 160.538 360.929 160.929C360.538 161.319 360.538 161.953 360.929 162.343L366.586 168L360.929 173.657C360.538 174.047 360.538 174.681 360.929 175.071C361.319 175.462 361.953 175.462 362.343 175.071L368.707 168.707ZM345 169L368 169L368 167L345 167L345 169Z" fill="black"/>
              <path className="arrow" d="M368.707 138.707C369.098 138.317 369.098 137.683 368.707 137.293L362.343 130.929C361.953 130.538 361.319 130.538 360.929 130.929C360.538 131.319 360.538 131.953 360.929 132.343L366.586 138L360.929 143.657C360.538 144.047 360.538 144.681 360.929 145.071C361.319 145.462 361.953 145.462 362.343 145.071L368.707 138.707ZM345 139L368 139L368 137L345 137L345 139Z" fill="black"/>
              <path d="M161 194C184.748 194 204 174.748 204 151C204 127.252 184.748 108 161 108C137.252 108 118 127.252 118 151C118 174.748 137.252 194 161 194Z" fill="#2E5A35"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '12.04px', letterSpacing: '0em' }}><tspan x="119.791" y="150.838">CustomerRoot</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '10.32px', letterSpacing: '0em' }}><tspan x="147.635" y="163.333">(root)</tspan></text>
              <path d="M408 185.5C426.778 185.5 442 170.278 442 151.5C442 132.722 426.778 117.5 408 117.5C389.222 117.5 374 132.722 374 151.5C374 170.278 389.222 185.5 408 185.5Z" fill="black"/>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '9.35px', letterSpacing: '0em' }}><tspan x="376.727" y="150.379">WishListEntity</tspan></text>
              <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '8.5px', letterSpacing: '0em' }}><tspan x="393.173" y="163.17">(entity)</tspan></text>
              <circle cx="221.5" cy="31.5" r="7.5" fill="#00E725"/>
              <path d="M225.192 29.1845C225.407 29.4005 225.407 29.7513 225.192 29.9673L220.789 34.3907C220.574 34.6067 220.225 34.6067 220.01 34.3907L217.808 32.179C217.593 31.963 217.593 31.6122 217.808 31.3962C218.023 31.1803 218.372 31.1803 218.587 31.3962L220.4 33.2157L224.415 29.1845C224.629 28.9685 224.979 28.9685 225.194 29.1845H225.192Z" fill="white"/>
              <circle cx="221.5" cy="139.5" r="7.5" fill="#00E725"/>
              <path d="M225.192 137.185C225.407 137.401 225.407 137.751 225.192 137.967L220.789 142.391C220.574 142.607 220.225 142.607 220.01 142.391L217.808 140.179C217.593 139.963 217.593 139.612 217.808 139.396C218.023 139.18 218.372 139.18 218.587 139.396L220.4 141.216L224.415 137.185C224.629 136.969 224.979 136.969 225.194 137.185H225.192Z" fill="white"/>
              <circle cx="221.5" cy="167.5" r="7.5" fill="#E65361"/>
              <path fillRule="evenodd" clipRule="evenodd" d="M225.287 171.047C224.893 171.441 224.255 171.441 223.861 171.047L217.953 165.139C217.559 164.745 217.559 164.107 217.953 163.713C218.347 163.319 218.985 163.319 219.379 163.713L225.287 169.621C225.681 170.015 225.681 170.653 225.287 171.047Z" fill="white"/>
              <path fillRule="evenodd" clipRule="evenodd" d="M217.713 171.108C217.319 170.714 217.319 170.075 217.713 169.682L223.621 163.774C224.015 163.38 224.653 163.38 225.047 163.774C225.441 164.168 225.441 164.806 225.047 165.2L219.139 171.108C218.745 171.501 218.107 171.501 217.713 171.108Z" fill="white"/>
              <circle cx="475.5" cy="152.5" r="20.5" fill="#E65361"/>
              <path fillRule="evenodd" clipRule="evenodd" d="M485.851 162.195C484.775 163.271 483.03 163.271 481.953 162.195L465.805 146.047C464.729 144.97 464.729 143.225 465.805 142.149C466.882 141.073 468.627 141.073 469.703 142.149L485.851 158.297C486.928 159.373 486.928 161.118 485.851 162.195Z" fill="white"/>
              <path fillRule="evenodd" clipRule="evenodd" d="M465.149 162.361C464.073 161.285 464.073 159.54 465.149 158.463L481.297 142.315C482.373 141.239 484.118 141.239 485.195 142.315C486.271 143.392 486.271 145.137 485.195 146.213L469.047 162.361C467.97 163.437 466.225 163.437 465.149 162.361Z" fill="white"/>
              <circle cx="221.5" cy="59.5" r="7.5" fill="#00E725"/>
              <path d="M225.192 57.1845C225.407 57.4005 225.407 57.7513 225.192 57.9673L220.789 62.3907C220.574 62.6067 220.225 62.6067 220.01 62.3907L217.808 60.179C217.593 59.963 217.593 59.6122 217.808 59.3962C218.023 59.1803 218.372 59.1803 218.587 59.3962L220.4 61.2157L224.415 57.1845C224.629 56.9685 224.979 56.9685 225.194 57.1845H225.192Z" fill="white"/>
              <circle cx="475" cy="40" r="20" fill="#00E725"/>
              <path d="M484.845 33.8253C485.418 34.4013 485.418 35.3366 484.845 35.9126L473.104 47.7085C472.53 48.2845 471.599 48.2845 471.026 47.7085L465.155 41.8106C464.582 41.2346 464.582 40.2992 465.155 39.7232C465.728 39.1473 466.659 39.1473 467.233 39.7232L472.067 44.5752L482.772 33.8253C483.345 33.2493 484.276 33.2493 484.85 33.8253H484.845Z" fill="white"/>
            </g>

            {/* Step 6 elements */}
            <g data-step="6"  style={{ transform: 'translateX(150px) translateY(185px) scale(1.8)' }}>
              
              <g data-transition="opacity5to6" opacity="0">
                <rect y="7" width="146" height="194" rx="14" fill="#ABFFAF"/>
                <rect x="171" y="7" width="146" height="194" rx="14" fill="#FFABAD"/>
                <path d="M83 193C100.673 193 115 178.673 115 161C115 143.327 100.673 129 83 129C65.3269 129 51 143.327 51 161C51 178.673 65.3269 193 83 193Z" fill="#2E5A35"/>
                <path d="M235 193C252.673 193 267 178.673 267 161C267 143.327 252.673 129 235 129C217.327 129 203 143.327 203 161C203 178.673 217.327 193 235 193Z" fill="#2E5A35"/>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '8.96px', letterSpacing: '0em' }}><tspan x="61.0451" y="160.798">OrderRoot</tspan></text>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '8.96px', letterSpacing: '0em' }}><tspan x="213.045" y="160.798">OrderRoot</tspan></text>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '7.68px', letterSpacing: '0em' }}><tspan x="73.1349" y="170.213">(root)</tspan></text>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '7.68px', letterSpacing: '0em' }}><tspan x="225.135" y="170.213">(root)</tspan></text>
                <circle cx="298.5" cy="25.5" r="13.5" fill="#E65361"/>
                <path fillRule="evenodd" clipRule="evenodd" d="M305.317 31.8843C304.608 32.5931 303.459 32.5931 302.75 31.8843L292.116 21.2503C291.407 20.5415 291.407 19.3923 292.116 18.6834C292.825 17.9746 293.974 17.9746 294.683 18.6834L305.317 29.3175C306.025 30.0263 306.025 31.1755 305.317 31.8843Z" fill="white"/>
                <path fillRule="evenodd" clipRule="evenodd" d="M291.683 31.9938C290.975 31.285 290.975 30.1358 291.683 29.427L302.317 18.7929C303.026 18.0841 304.175 18.0841 304.884 18.7929C305.593 19.5018 305.593 20.651 304.884 21.3598L294.25 31.9938C293.541 32.7026 292.392 32.7026 291.683 31.9938Z" fill="white"/>
                <circle cx="19.5" cy="25.5" r="13.5" fill="#00E725"/>
                <path d="M26.1454 21.3321C26.5324 21.7209 26.5324 22.3523 26.1454 22.7411L18.2199 30.7033C17.8329 31.0921 17.2044 31.0921 16.8174 30.7033L12.8546 26.7222C12.4676 26.3334 12.4676 25.702 12.8546 25.3132C13.2416 24.9245 13.8701 24.9245 14.2571 25.3132L17.5202 28.5883L24.7461 21.3321C25.133 20.9433 25.7615 20.9433 26.1485 21.3321H26.1454Z" fill="white"/>
                <path d="M82.0001 51.0101C82.0056 51.5624 82.4578 52.0055 83.0101 52L92.0096 51.909C92.5619 51.9035 93.0051 51.4513 92.9995 50.899C92.9939 50.3467 92.5417 49.9036 91.9894 49.9091L83.9898 49.99L83.909 41.9904C83.9035 41.4381 83.4513 40.9949 82.899 41.0005C82.3467 41.0061 81.9036 41.4583 81.9091 42.0106L82.0001 51.0101ZM131.286 0.300071L82.2858 50.3001L83.7142 51.6999L132.714 1.69993L131.286 0.300071Z" fill="black"/>
                <path d="M83.5444 126.048C83.2438 126.349 82.7562 126.349 82.4556 126.048L77.5555 121.148C77.2548 120.847 77.2548 120.36 77.5555 120.059C77.8562 119.758 78.3437 119.758 78.6444 120.059L83 124.415L87.3556 120.059C87.6563 119.758 88.1438 119.758 88.4445 120.059C88.7452 120.36 88.7452 120.847 88.4445 121.148L83.5444 126.048ZM83.77 84V125.504H82.23V84H83.77Z" fill="black"/>
                <path d="M235.544 125.048C235.244 125.349 234.756 125.349 234.456 125.048L229.556 120.148C229.255 119.847 229.255 119.36 229.556 119.059C229.856 118.758 230.344 118.758 230.644 119.059L235 123.415L239.356 119.059C239.656 118.758 240.144 118.758 240.444 119.059C240.745 119.36 240.745 119.847 240.444 120.148L235.544 125.048ZM235.77 83V124.504H234.23V83H235.77Z" fill="black"/>
                <path d="M236 51.0101C235.994 51.5624 235.542 52.0055 234.99 52L225.99 51.909C225.438 51.9035 224.995 51.4513 225.001 50.899C225.006 50.3467 225.458 49.9036 226.011 49.9091L234.01 49.99L234.091 41.9904C234.097 41.4381 234.549 40.9949 235.101 41.0005C235.653 41.0061 236.096 41.4583 236.091 42.0106L236 51.0101ZM186.714 0.300071L235.714 50.3001L234.286 51.6999L185.286 1.69993L186.714 0.300071Z" fill="black"/>
                <rect x="189" y="54" width="88" height="24" rx="7" fill="#FF4D4D"/>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="210.089" y="73.6818">Root Command</tspan></text>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="216.192" y="64.6818">CreateOrder</tspan></text>
                <rect x="43" y="56" width="88" height="24" rx="7" fill="#1FA2FF"/>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', letterSpacing: '0em' }}><tspan x="70.4746" y="75.6818">Root Query</tspan></text>
                <text fill="white" style={{ whiteSpace: 'pre', fontFamily: 'Arial', fontSize: '6px', fontWeight: 'bold', letterSpacing: '0em' }}><tspan x="73.2285" y="66.6818">FindOrder</tspan></text>
              </g>

              <g data-transition="5to6" style={{ transform: 'translateX(-50px) scale(.777)' }} opacity="0" >
                <path d="M161 86C184.748 86 204 66.7482 204 43C204 19.2518 184.748 0 161 0C137.252 0 118 19.2518 118 43C118 66.7482 137.252 86 161 86Z" fill="#2E5A35"></path>
                <text fill="white" style={{ whiteSpace: "pre", fontFamily: "Arial", fontSize: "12.04px", letterSpacing: "0em" }}><tspan x="119.791" y="42.8382">CustomerRoot</tspan></text>
                <text fill="white" style={{ whiteSpace: "pre", fontFamily: "Arial", fontSize: "10.32px", letterSpacing: "0em" }}><tspan x="147.635" y="55.3328">(root)</tspan></text>
              </g>
            </g>

          </g>
        </svg>
      </div>
      
      <div style={{ 
          flex: '0 0 40%', 
          display: 'flex', 
          flexDirection: 'column', 
          gap: '30px', 
          padding: '20px', 
          paddingRight: '80px',
          justifyContent: 'center'
      }}>
        <div style={dropdownContainerStyle}>
          <h1 style={{
            fontSize: '28px',
            fontWeight: 'bold',
            marginBottom: '16px',
            color: 'var(--ifm-color-emphasis-900)'
          }}>
            {stepMetadata[currentStep - 1].title}
          </h1>
          <select 
            value={currentStep}
            onChange={handleStepChange}
            style={dropdownStyle}
            disabled={isAnimating}
          >
            {Array.from({ length: totalSteps }, (_, i) => i + 1).map((step) => (
              <option key={step} value={step}>
                {stepMetadata[step - 1].title}
              </option>
            ))}
          </select>
        </div>

        <div style={{ 
          fontSize: '16px',
          lineHeight: '1.6',
          color: 'var(--ifm-color-emphasis-800)',
          marginBottom: '20px'
        }}>
          {stepMetadata[currentStep - 1].description}
        </div>

        <div style={{ display: 'flex', flexDirection: 'row', gap: '10px', width: '100%' }}>
          <button
            onClick={handleBack}
            style={{ 
              ...currentStep === 1 || isAnimating ? disabledButtonStyle : buttonStyle,
              flex: 1
            }}
            disabled={currentStep === 1 || isAnimating}
          >
            Back
          </button>
          <button
            onClick={handleNext}
            style={{ 
              ...currentStep === totalSteps || isAnimating ? disabledButtonStyle : buttonStyle,
              flex: 1
            }}
            disabled={currentStep === totalSteps || isAnimating}
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
} 