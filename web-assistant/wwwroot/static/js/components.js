import {html, css, LitElement} from 'https://cdn.jsdelivr.net/gh/lit/dist@2/core/lit-core.min.js';

const keywords = { 
  "FillTextField": { 
    "name": "Textfeld Ausfüllen", 
    "args": ["Lokator", "Inhalt"], 
    "doc": "Fülle ein Textfeld aus"
  }
};

export class KeywordCall extends LitElement {
  static styles = css`p { color: blue }`;

  static properties = {
    name: {type: String},
  };

  constructor() {
    super();
    this.name = 'Keyword';
  }

  render() {
    const keyword = keywords[this.name]
    const args = [];
    for (const arg of keyword.args) {
      args.push(html`<div style="display: inline; padding-right: 20px"><input type="text" name="${arg}" placeholder="${arg}"></div>`);
    }
 
    return html`<button class="primary center" onclick="callKeyword('${this.name}')" style="font-size:1rem;">▶</button><p style="display: inline; padding-left: 20px; padding-right: 20px">${keyword.name}</p>${args} <div style="font-size:2rem;width:100%;display: inline;">🖼</div>`;
  }
}

customElements.define('keyword-call', KeywordCall);
