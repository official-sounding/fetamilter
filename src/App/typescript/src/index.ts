import { ThemeToggle } from "./modules/theme-toggle.js";

customElements.define('theme-toggle', ThemeToggle);

document.querySelectorAll('time').forEach($e => {
  const date = new Date($e.dateTime);
  const type = $e.getAttribute('type');

  switch(type) {
    case 'just-date':
        $e.innerHTML = date.toLocaleDateString();
        break;
    case 'logline':
        $e.innerHTML = date.toLocaleString(undefined, { dateStyle: 'short', timeStyle: 'short'});
        break;
    case 'post-title':
        $e.innerHTML = date.toLocaleString(undefined, { dateStyle: 'long', timeStyle: 'short'});
        break;
    default:
        $e.innerHTML = date.toLocaleString();
  }
});
