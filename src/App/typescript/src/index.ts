import { CommentFavoriteButton } from './modules/comment-favorite-button.js';
import { PostFavoriteButton } from './modules/post-favorite-button.js';
import { ThemeToggle } from './modules/theme-toggle.js';

customElements.define('theme-toggle', ThemeToggle);
customElements.define('post-favorite-button', PostFavoriteButton);
customElements.define('comment-favorite-button', CommentFavoriteButton);

for (const $e of document.querySelectorAll('time')) {
    const date = new Date($e.dateTime);
    const type = $e.getAttribute('type');

    switch (type) {
        case 'just-date':
            $e.innerHTML = date.toLocaleDateString();
            break;
        case 'logline':
            $e.innerHTML = date.toLocaleString(undefined, {
                dateStyle: 'short',
                timeStyle: 'short',
            });
            break;
        case 'post-title':
            $e.innerHTML = date.toLocaleString(undefined, {
                dateStyle: 'long',
                timeStyle: 'short',
            });
            break;
        default:
            $e.innerHTML = date.toLocaleString();
    }
}
