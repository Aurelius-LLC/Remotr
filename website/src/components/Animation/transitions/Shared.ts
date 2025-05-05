import anime from 'animejs';
import { ROOT_OSCILLATION_DURATION, COLOR_RESET_DURATION } from './types';

let customerRootAnimation: anime.AnimeInstance | null = null;

export const startRootOscillation = (svg: SVGSVGElement) => {
  const customerRoot = svg.querySelector('g[data-aggregate="customer"] > circle[cx="0"][cy="-70"]');
  if (customerRoot) {
    customerRootAnimation = anime({
      targets: customerRoot,
      fill: ['#2E5A35', '#1A331F', '#2E5A35'],
      duration: ROOT_OSCILLATION_DURATION,
      easing: 'easeInOutSine',
      loop: true,
      direction: 'alternate'
    });
  }
};

export const stopRootOscillation = (svg: SVGSVGElement) => {
  if (customerRootAnimation) {
    customerRootAnimation.pause();
    const customerRoot = svg.querySelector('g[data-aggregate="customer"] > circle[cx="0"][cy="-70"]');
    if (customerRoot) {
      anime({
        targets: customerRoot,
        fill: '#2E5A35',
        duration: COLOR_RESET_DURATION
      });
    }
  }
};