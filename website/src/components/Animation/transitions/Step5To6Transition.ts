import anime from 'animejs';
import { Transition, TRANSITION_DURATION } from './types';

export const Step5To6Transition: Transition = {
    forward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        // Get the transition element
        const transitionElement = svg.querySelector('g[data-transition="5to6"]');
        
        // Animate the translation
        if (transitionElement) {
          anime({
            targets: transitionElement,
            translateX: [-50, 34],
            translateY: [0, -70],
            scale: 0.777,
            opacity: 1,
            duration: skipAnimation ? 0 : TRANSITION_DURATION,
            easing: 'easeInOutQuad'
          });
        }

        // Fade out step 5 elements
        anime({
          targets: svg.querySelector('g[data-step="5"]'),
          opacity: [1, 0],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad'
        });

        // Fade in step 6 elements
        anime({
          targets: svg.querySelector('g[data-transition="opacity5to6"]'),
          opacity: [0, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => resolve()
        });
      });
    },
    backward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
      return new Promise((resolve) => {
        const transitionElement = svg.querySelector('g[data-transition="5to6"]');

        // Animate the translation back
        if (transitionElement) {
          anime({
            targets: transitionElement,
            translateX: [34, -50],
            translateY: [-70, 0],
            scale: 0.777,
            opacity: 0,
            duration: skipAnimation ? 0 : TRANSITION_DURATION,
            easing: 'easeInOutQuad'
          });
        }

        // Fade out step 6 elements
        anime({
          targets: svg.querySelector('g[data-transition="opacity5to6"]'),
          opacity: [1, 0],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad'
        });

        // Fade in step 5 elements
        anime({
          targets: svg.querySelector('g[data-step="5"]'),
          opacity: [0, 1],
          duration: skipAnimation ? 0 : TRANSITION_DURATION,
          easing: 'easeInOutQuad',
          complete: () => resolve()
        });
      });
    }
}; 