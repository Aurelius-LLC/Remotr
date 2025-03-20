import anime from 'animejs';
import { Transition, TRANSITION_DURATION } from './types';
import { startRootOscillation, stopRootOscillation } from './Shared';


export const Step2To3Transition: Transition = {
    forward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        stopRootOscillation(svg);
        
        // Animate customer aggregate to slightly smaller scale
        anime({
          targets: svg.querySelector('g[data-aggregate="customer"]'),
          scale: [1.8, 1.5],
          translateX: [400, 250],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad'
        });
  
        // Fade in commands
        anime({
          targets: svg.querySelector('g[data-commands="customer"]'),
          opacity: [0, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => resolve()
        });
      });
    },
    backward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        // Animate customer aggregate back to larger scale
        anime({
          targets: svg.querySelector('g[data-aggregate="customer"]'),
          scale: [1.2, 1.8],
          translateX: [250, 400],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad'
        });
  
        // Fade out commands
        anime({
          targets: svg.querySelector('g[data-commands="customer"]'),
          opacity: [1, 0],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => {
            startRootOscillation(svg);
            resolve();
          }
        });
      });
    }
  };