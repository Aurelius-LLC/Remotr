import anime from 'animejs';
import { Transition, TRANSITION_DURATION } from './types';

export const Step3To4Transition: Transition = {
    forward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        // Fade out customer aggregate and commands
        anime({
          targets: [
            svg.querySelector('g[data-aggregate="customer"]'),
            svg.querySelector('g[data-commands="customer"]')
          ],
          opacity: [1, 0],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad'
        });
  
        // Show and animate step 4 elements
        anime({
          targets: svg.querySelector('g[data-step="4"]'),
          opacity: [0, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad'
        });
  
        // Animate the root element's position
        anime({
          targets: svg.querySelector('g[data-step="4root"]'),
          translateX: [200, 250],
          translateY: [350, 200],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => resolve()
        });
      });
    },
    backward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
  
        // Animate the root element back to its original position
        anime({
          targets: svg.querySelector('g[data-step="4root"]'),
          translateX: [250, 200],
          translateY: [200, 350],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => {
                // Fade in customer aggregate and commands
                anime({
                    targets: [
                    svg.querySelector('g[data-aggregate="customer"]'),
                    svg.querySelector('g[data-commands="customer"]')
                    ],
                    opacity: [0, 1],
                    duration: skipAnimation ? 0 : TRANSITION_DURATION,
                    easing: 'easeInOutQuad',
                    complete: () => resolve()
                });

                // Fade out step 4 elements
                anime({
                    targets: svg.querySelector('g[data-step="4"]'),
                    opacity: [1, 0],
                    duration: skipAnimation ? 0 : TRANSITION_DURATION,
                    easing: 'easeInOutQuad'
                });
            }
        });
      });
    }
  }; 