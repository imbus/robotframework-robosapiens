import {html, css, LitElement} from 'https://cdn.jsdelivr.net/gh/lit/dist@2/core/lit-core.min.js';
import api from './api.json' assert { type: 'json' };

const keywords = api.keywords;

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
    for (const arg of Object.keys(keyword.args)) {
      args.push(html`<div style="display: inline; padding-right: 20px"><input type="text" name="${arg}" placeholder="${arg}"></div>`);
    }
 
    return html`<div class="row"><button class="primary center" onclick="callKeyword('${this.name}')" style="font-size:1rem;">▶</button><p style="display: inline; padding-left: 20px; padding-right: 20px">${keyword.name}</p>${args} <div style="font-size:2rem;width:100%;display: inline;">🖼</div></div>`;
  }
}

export class KeywordList extends LitElement {
  static styles = css`p { color: blue }`;

  static properties = {
  };

  constructor() {
    super();
  }

  render() {
    const keywords = [];
    for (const keyword of Object.keys(api.keywords)) {
      keywords.push(html`<div class="list-group-item" id="${keyword}">${api.keywords[keyword].name}</div>`);
    }
 
    return html`<div class="list-group">${keywords}</div>`;
  }
}

customElements.define('keyword-call', KeywordCall);
customElements.define('keyword-list', KeywordList);
