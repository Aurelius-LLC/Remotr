import React, { useEffect, useRef, useState } from 'react';
import anime from 'animejs';

export default function OverviewAnimation(): React.JSX.Element {
  const svgRef = useRef<SVGSVGElement>(null);
  const customerRootAnimation = useRef<anime.AnimeInstance | null>(null);
  const [currentStep, setCurrentStep] = useState(1);
  const [isAnimating, setIsAnimating] = useState(false);
  const totalSteps = 3;

  // Animation constants
  const TRANSITION_DURATION = 300;  // Duration in milliseconds for transition animations
  const ROOT_OSCILLATION_DURATION = 1000;  // Duration for root color oscillation
  const COLOR_RESET_DURATION = 300;  // Duration for resetting root color

  // Function to start the root color oscillation
  const startRootOscillation = () => {
    if (svgRef.current) {
      const customerRoot = svgRef.current.querySelector('g[data-aggregate="customer"] > circle[cx="0"][cy="-70"]');
      if (customerRoot) {
        customerRootAnimation.current = anime({
          targets: customerRoot,
          fill: ['#2E5A35', '#1A331F', '#2E5A35'],
          duration: ROOT_OSCILLATION_DURATION,
          easing: 'easeInOutSine',
          loop: true,
          direction: 'alternate'
        });
      }
    }
  };

  // Function to stop the root color oscillation
  const stopRootOscillation = () => {
    if (customerRootAnimation.current) {
      customerRootAnimation.current.pause();
      // Reset the color back to original
      if (svgRef.current) {
        const customerRoot = svgRef.current.querySelector('g[data-aggregate="customer"] > circle[cx="0"][cy="-70"]');
        if (customerRoot) {
          anime({
            targets: customerRoot,
            fill: '#2E5A35',
            duration: COLOR_RESET_DURATION
          });
        }
      }
    }
  };

  // Function to animate from step 1 (initial state) to step 2 (Customer aggregate focused)
  const animateToStep2 = () => {
    setIsAnimating(true);
    if (svgRef.current) {
      // Animate customer aggregate to center and scale up
      anime({
        targets: svgRef.current.querySelector('g[data-aggregate="customer"]'),
        translateY: [180, 350],
        scale: [1, 1.8],
        duration: TRANSITION_DURATION,
        easing: 'easeInOutQuad',
        complete: () => {
          startRootOscillation();
          setIsAnimating(false);
        }
      });

      // Fade out store and order aggregates
      anime({
        targets: [
          svgRef.current.querySelector('g[data-aggregate="store"]'),
          svgRef.current.querySelector('g[data-aggregate="order"]')
        ],
        opacity: [1, 0],
        duration: TRANSITION_DURATION * 0.8,
        easing: 'easeInOutQuad'
      });
    }
  };

  // Function to animate from step 2 back to step 1 (initial state)
  const animateToStep1 = () => {
    setIsAnimating(true);
    stopRootOscillation();
    
    if (svgRef.current) {
      // Animate customer aggregate back to original position
      anime({
        targets: svgRef.current.querySelector('g[data-aggregate="customer"]'),
        translateY: [350, 180],
        scale: [1.8, 1],
        duration: TRANSITION_DURATION,
        easing: 'easeInOutQuad',
        complete: () => setIsAnimating(false)
      });

      // Fade in store and order aggregates
      anime({
        targets: [
          svgRef.current.querySelector('g[data-aggregate="store"]'),
          svgRef.current.querySelector('g[data-aggregate="order"]')
        ],
        opacity: [0, 1],
        duration: TRANSITION_DURATION * 0.8,
        easing: 'easeInOutQuad'
      });
    }
  };

  // Function to animate from step 2 to step 3 (Customer aggregate at medium scale)
  const animateToStep3 = () => {
    setIsAnimating(true);
    stopRootOscillation();
    
    if (svgRef.current) {
      // Animate customer aggregate to slightly smaller scale
      anime({
        targets: svgRef.current.querySelector('g[data-aggregate="customer"]'),
        scale: [1.8, 1.2],
        translateX: [400, 200],
        duration: TRANSITION_DURATION,
        easing: 'easeInOutQuad',
        complete: () => setIsAnimating(false)
      });
    }
  };

  // Function to animate from step 3 back to step 2 (Customer aggregate back to larger scale)
  const animateBackToStep2 = () => {
    setIsAnimating(true);
    if (svgRef.current) {
      // Animate customer aggregate back to larger scale
      anime({
        targets: svgRef.current.querySelector('g[data-aggregate="customer"]'),
        scale: [1.2, 1.8],
        translateX: [200, 400],
        duration: TRANSITION_DURATION,
        easing: 'easeInOutQuad',
        complete: () => {
          startRootOscillation();
          setIsAnimating(false);
        }
      });
    }
  };

  const handleNext = () => {
    if (currentStep < totalSteps && !isAnimating) {
      setCurrentStep(currentStep + 1);
      if (currentStep === 1) {
        animateToStep2();
      } else if (currentStep === 2) {
        animateToStep3();
      }
    }
  };

  const handleBack = () => {
    if (currentStep > 1 && !isAnimating) {
      setCurrentStep(currentStep - 1);
      if (currentStep === 2) {
        animateToStep1();
      } else if (currentStep === 3) {
        animateBackToStep2();
      }
    }
  };

  // Button styles
  const buttonStyle = {
    padding: '10px 20px',
    margin: '0 10px',
    backgroundColor: '#2E5A35',
    color: 'white',
    border: 'none',
    borderRadius: '4px',
    cursor: 'pointer',
    fontSize: '16px',
  };

  const disabledButtonStyle = {
    ...buttonStyle,
    backgroundColor: '#cccccc',
    cursor: 'not-allowed',
  };

  return (
    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
      <svg 
        ref={svgRef} 
        width="800" 
        height="680" 
        viewBox="0 0 800 680" 
        xmlns="http://www.w3.org/2000/svg"
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
            <text x="0" y="-170" fontFamily="Arial" fontSize="18" textAnchor="middle" fontWeight="bold">Customer Aggregate</text>
            
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
          
          <g transform="translate(0, 0)">
            <rect x="131" width="88" height="24" rx="7" fill="#FF4D4D"/>
            <text x="160.004" y="19.6818" fill="white" fontFamily="Arial" fontSize="6">Command</text>
            <text x="144.143" y="10.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">CreateUserCommand</text>
            
            <rect x="131" y="26" width="88" height="24" rx="7" fill="#1FA2FF"/>
            <text x="165.39" y="45.6818" fill="white" fontFamily="Arial" fontSize="6">Query</text>
            <text x="155.324" y="36.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">GetUserQuery</text>
            
            <rect x="131" y="78" width="88" height="24" rx="7" fill="#1FA2FF"/>
            <text x="165.39" y="97.6818" fill="white" fontFamily="Arial" fontSize="6">Query</text>
            <text x="155.246" y="88.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">GetCartQuery</text>
            
            <rect x="131" y="52" width="88" height="24" rx="7" fill="#FF4D4D"/>
            <text x="160.004" y="71.6818" fill="white" fontFamily="Arial" fontSize="6">Command</text>
            <text x="136.162" y="62.6818" fill="white" fontFamily="Arial" fontSize="6" fontWeight="bold">ProcessPaymentCommand</text>
            
            <path d="M9.18627 32.4188C8.86526 32.8682 8.96935 33.4927 9.41876 33.8137L16.7424 39.0449C17.1918 39.3659 17.8163 39.2618 18.1373 38.8124C18.4583 38.363 18.3543 37.7384 17.9048 37.4174L11.395 32.7675L16.0449 26.2576C16.3659 25.8082 16.2618 25.1837 15.8124 24.8627C15.363 24.5417 14.7384 24.6457 14.4174 25.0952L9.18627 32.4188ZM123.836 13.0136L9.8356 32.0136L10.1644 33.9864L124.164 14.9864L123.836 13.0136Z" fill="black"/>
            <path d="M0.264971 43.322C-0.109502 43.7279 -0.0839922 44.3605 0.321949 44.735L6.93715 50.8374C7.34309 51.2119 7.97574 51.1864 8.35021 50.7804C8.72468 50.3745 8.69917 49.7418 8.29323 49.3674L2.41306 43.943L7.83741 38.0628C8.21188 37.6569 8.18637 37.0243 7.78043 36.6498C7.37449 36.2753 6.74183 36.3008 6.36736 36.7068L0.264971 43.322ZM124.96 38.0008L0.959703 43.0008L1.04028 44.9992L125.04 39.9992L124.96 38.0008Z" fill="black"/>
            <path d="M0.316252 59.2703C-0.0867593 59.6479 -0.107339 60.2807 0.270286 60.6838L6.42404 67.2512C6.80166 67.6542 7.43449 67.6748 7.8375 67.2972C8.24051 66.9195 8.26109 66.2867 7.88347 65.8837L2.41347 60.046L8.2512 54.576C8.65421 54.1983 8.67479 53.5655 8.29717 53.1625C7.91954 52.7595 7.28671 52.7389 6.8837 53.1165L0.316252 59.2703ZM124.033 63.0005L1.03251 59.0005L0.967499 60.9995L123.967 64.9995L124.033 63.0005Z" fill="black"/>
            <path d="M4.39461 72.2041C3.95503 72.5384 3.86972 73.1658 4.20407 73.6054L9.65257 80.7688C9.98692 81.2083 10.6143 81.2936 11.0539 80.9593C11.4935 80.6249 11.5788 79.9976 11.2444 79.558L6.40132 73.1905L12.7688 68.3474C13.2083 68.0131 13.2936 67.3857 12.9593 66.9461C12.6249 66.5065 11.9976 66.4212 11.558 66.7556L4.39461 72.2041ZM121.986 87.8977L5.13473 72.0091L4.86527 73.9909L121.717 89.8794L121.986 87.8977Z" fill="black"/>
          </g>
          
          {/* Store Aggregate - positioned at the bottom left of the triangle */}
          <g data-aggregate="store" style={{ transform: 'translateX(200px) translateY(440px)' }}>
            {/* Main aggregate bubble */}
            <circle cx="0" cy="0" r="150" fill="url(#storeGradient)" opacity="0.8"/>
            <text x="0" y="180" fontFamily="Arial" fontSize="18" textAnchor="middle" fontWeight="bold">Store Aggregate</text>
            
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
            <text x="0" y="180" fontFamily="Arial" fontSize="18" textAnchor="middle" fontWeight="bold">Order Aggregate</text>
            
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
        </g>
      </svg>

      <div style={{ display: 'flex', marginTop: '20px' }}>
        <button
          onClick={handleBack}
          style={currentStep === 1 || isAnimating ? disabledButtonStyle : buttonStyle}
          disabled={currentStep === 1 || isAnimating}
        >
          Back
        </button>
        <button
          onClick={handleNext}
          style={currentStep === totalSteps || isAnimating ? disabledButtonStyle : buttonStyle}
          disabled={currentStep === totalSteps || isAnimating}
        >
          Next
        </button>
      </div>
    </div>
  );
} 