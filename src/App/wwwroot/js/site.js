// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
class PostFavoriteButton extends HTMLElement {
    static observedAttributes = ["post"];

    btn;
    msg;

    favorited = false;

    constructor() {
        // Always call super first in constructor
        super();

        const shadow = this.attachShadow({ mode: "open" });

        this.btn = document.createElement("button");
        this.msg = document.createElement("span");

        this.btn.className = "btn-link";
        this.msg.className = "fav-details";

        this.btn.addEventListener("click", () => this.toggleFavorite());
        this.setBtnText();

        const sheet = new CSSStyleSheet();
        sheet.replaceSync(
            ".btn-link { background-color: transparent; border: 0; text-decoration: underline; color: var(--link-color) }"
        );

        shadow.adoptedStyleSheets.push(sheet);
        shadow.appendChild(this.btn);
        shadow.appendChild(this.msg);
    }

    async toggleFavorite() {
        const postNum = this.getAttribute("post");
        if (!postNum) {
            return;
        }

        const method = this.favorited ? "DELETE" : "POST";
        const result = await fetch(`/${postNum}/favorite`, { method });
        if (!result.ok) {
            return;
        }

        this.favorited = !this.favorited;
        this.setBtnText();
        const data = await result.json();
        if (data.actionSuccessful) {
            this.msg.innerText = this.favorited
                ? "favorite added"
                : "favorite removed";
        } else {
            this.msg.innerText = this.favorited
                ? "favorite already added"
                : "favorite already removed";
        }
    }

    setBtnText() {
        this.btn.innerText = this.favorited ? "-" : "+";
    }
}

customElements.define("post-favorite-button", PostFavoriteButton);
