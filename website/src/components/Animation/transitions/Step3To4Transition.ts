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
          duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
          easing: 'easeInOutQuad'
        });
  
        // Show and animate step 4 elements
        anime({
          targets: svg.querySelector('g[data-step="4"]'),
          opacity: [0, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
          easing: 'easeInOutQuad'
        });
  
        // Animate the root element's position
        anime({
          targets: svg.querySelector('g[data-step="4root"]'),
          translateX: [150, 250],
          translateY: [150, 200],
          duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
          easing: 'easeInOutQuad'
        });

        // Animate the entity element's position
        anime({
          targets: svg.querySelector('g[data-step="4entity"]'),
          translateX: [150, 520],
          translateY: [150, 90],
          duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
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
          translateX: [250, 150],
          translateY: [200, 150],
          duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
          easing: 'easeInOutQuad'
        });

        // Animate the entity element back to its original position
        anime({
          targets: svg.querySelector('g[data-step="4entity"]'),
          translateX: [520, 150],
          translateY: [90, 150],
          duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
          easing: 'easeInOutQuad',
          complete: () => {
                // Fade in customer aggregate and commands
                anime({
                    targets: [
                    svg.querySelector('g[data-aggregate="customer"]'),
                    svg.querySelector('g[data-commands="customer"]')
                    ],
                    opacity: [0, 1],
                    duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
                    easing: 'easeInOutQuad',
                    complete: () => resolve()
                });

                // Fade out step 4 elements
                anime({
                    targets: svg.querySelector('g[data-step="4"]'),
                    opacity: [1, 0],
                    duration: skipAnimation ? 0 : TRANSITION_DURATION*1.5,
                    easing: 'easeInOutQuad'
                });
            }
        });
      });
    }
  }; 