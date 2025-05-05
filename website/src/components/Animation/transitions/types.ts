import { AnimeInstance } from 'animejs';

export interface Transition {
  forward: (svg: SVGSVGElement, skipAnimation?: boolean) => Promise<void>;
  backward: (svg: SVGSVGElement, skipAnimation?: boolean) => Promise<void>;
  cleanup?: () => void;
}

export interface TransitionState {
  currentStep: number;
  isAnimating: boolean;
  activeAnimations: AnimeInstance[];
}

export const TRANSITION_DURATION = 300;  // Duration in milliseconds for transition animations
export const ROOT_OSCILLATION_DURATION = 1000;  // Duration for root color oscillation
export const COLOR_RESET_DURATION = 300;  // Duration for resetting root color 