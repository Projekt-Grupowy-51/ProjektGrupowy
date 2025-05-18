type Listener = (count: number) => void;

let counter = 0;
const listeners: Set<Listener> = new Set();

function notify(): void {
  listeners.forEach((fn) => fn(counter));
}

export function subscribe(fn: Listener): () => void {
  listeners.add(fn);
  fn(counter);
  return () => listeners.delete(fn);
}

export function startRequest(): void {
  counter++;
  notify();
}

export function endRequest(): void {
  counter = Math.max(0, counter - 1);
  notify();
}
