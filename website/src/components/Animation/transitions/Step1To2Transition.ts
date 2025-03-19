import anime from 'animejs';
import { Transition, TRANSITION_DURATION } from './types';
import { startRootOscillation, stopRootOscillation } from './Shared';

let customerRootAnimation: anime.AnimeInstance | null = null;

export const Step1To2Transition: Transition = {
    forward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        // Animate customer aggregate to center and scale up
        anime({
          targets: svg.querySelector('g[data-aggregate="customer"]'),
          translateY: [180, 350],
          scale: [1, 1.8],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => {
            startRootOscillation(svg);
            resolve();
          }
        });
  
        // Fade out store and order aggregates
        anime({
          targets: [
            svg.querySelector('g[data-aggregate="store"]'),
            svg.querySelector('g[data-aggregate="order"]')
          ],
          opacity: [1, 0],
          duration: skipAnimation ? 0 : TRANSITION_DURATION * 0.8,
          easing: 'easeInOutQuad'
        });
      });
    },
    backward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        stopRootOscillation(svg);
        
        // Animate customer aggregate back to original position
        anime({
          targets: svg.querySelector('g[data-aggregate="customer"]'),
          translateY: [350, 180],
          scale: [1.8, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => resolve()
        });
  
        // Fade in store and order aggregates
        anime({
          targets: [
            svg.querySelector('g[data-aggregate="store"]'),
            svg.querySelector('g[data-aggregate="order"]')
          ],
          opacity: [0, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION * 0.8,
          easing: 'easeInOutQuad'
        });
      });
    },
    cleanup: () => {
      if (customerRootAnimation) {
        customerRootAnimation.pause();
        customerRootAnimation = null;
      }
    }
  };