import React, { useEffect, useRef } from 'react';
import anime from 'animejs';

interface AnimationBoxProps {
  boxColor?: string;
  finalColor?: string;
}

export default function AnimeExample({ 
  boxColor = '#6b52ae', 
  finalColor = '#ff4081' 
}: AnimationBoxProps): React.JSX.Element {
  const boxRef = useRef<HTMLDivElement>(null);
  
  const runAnimation = () => {
    if (boxRef.current) {
      // Reset position first
      anime.set(boxRef.current, {
        translateX: 0,
        rotate: 0,
        backgroundColor: boxColor
      });
      
      // Run the animation
      anime({
        targets: boxRef.current,
        translateX: 250,
        rotate: '1turn',
        backgroundColor: finalColor,
        scale: 1.2,
        duration: 1000,
        easing: 'easeInOutQuad'
      });
    }
  };
  
  return (
    <div className="anime-demo" style={{ padding: '20px 0' }}>
      <div 
        ref={boxRef} 
        style={{ 
          width: '60px', 
          height: '60px', 
          backgroundColor: boxColor,
          borderRadius: '4px'
        }}
      />
      <button 
        onClick={runAnimation}
        style={{
          marginTop: '15px',
          padding: '8px 16px',
          backgroundColor: '#1877F2',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
      >
        Animate
      </button>
    </div>
  );
} 