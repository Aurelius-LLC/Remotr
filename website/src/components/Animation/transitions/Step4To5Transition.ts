import anime from 'animejs';
import { Transition, TRANSITION_DURATION } from './types';

export const Step4To5Transition: Transition = {
  forward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
    return new Promise((resolve) => {
      // Fade out step 4 elements
      anime({
        targets: svg.querySelector('g[data-step="4"]'),
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
  },
  backward: async (svg: SVGSVGElement, skipAnimation: boolean = false) => {
    return new Promise((resolve) => {
      // Fade out step 5 elements
      anime({
        targets: svg.querySelector('g[data-step="5"]'),
        opacity: [1, 0],
        duration: skipAnimation ? 0 : TRANSITION_DURATION,
        easing: 'easeInOutQuad'
      });

      // Fade in step 4 elements
      anime({
        targets: svg.querySelector('g[data-step="4"]'),
        opacity: [0, 1],
        duration: skipAnimation ? 0 : TRANSITION_DURATION,
        easing: 'easeInOutQuad',
        complete: () => resolve()
      });
    });
  }
}; 