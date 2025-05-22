let counter = 0;
const listeners = new Set();
function notify() { listeners.forEach(fn => fn(counter)); }
export function subscribe(fn) {
  listeners.add(fn);
  fn(counter);
  return () => listeners.delete(fn);
}
export function startRequest() {
  counter++;
  notify();
}
export function endRequest() {
  counter = Math.max(0, counter - 1);
  notify();
}
