import api from '../api.json';
import {html} from 'code-tag';
import {Sortable, OnSpill} from 'sortablejs/modular/sortable.core.esm';

Sortable.mount(OnSpill);

const keywords_by_id = Object.fromEntries(new Map(api.keywords.map(keyword => [keyword.id, keyword])));

function keywordCall(id) {
    const keyword = keywords_by_id[id];
    const args = [
      '<div style="padding: 2px">', 
      keyword.args.map(arg => 
        html`<input type="text" name="${arg.id}" placeholder="${arg.name}" style="width: ${100/keyword.args.length-2}%; margin: 2px;">`
      ).join(''),  
      '</div>'
    ].join('');
 
    return html`
      <div class="row g-1">
        <div class="handle col-1" style="cursor: grab; width: auto; font-size:13px;">📌</div>
        <div id="${id}" class="col">
          <button class="primary center" onclick="callKeyword('${id}')" style="font-size:1rem; dispay:inline; border: 0px; padding: 0px; margin: 0px; background-color: #fff;">▶</button>
          <span>${keyword.name}</span>
          <!-- <span style="font-size:1rem;">🖼</span> -->
          ${args}
        </div>
      <div>
    `;
  }

function keywordList() {
  const keywords = api.keywords.map(keyword => 
    html`<div class="list-group-item" id="${keyword.id}" style="cursor: grab">${keyword.name}</div>`
  ).join('')

  return html`<div id="keyword-list" class="list-group">${keywords}</div>`;
}

const keywords = document.getElementById("keywords");
keywords.innerHTML = keywordList()

const keyword_list = document.getElementById("keyword-list");
new Sortable(
  keyword_list, 
  {
    group: 
    {
      name: 'shared',
      pull: 'clone',
      put: false
    },
    animation: 150,
    sort: false,
    onClone: function(event) {
      event.clone.id = event.item.id
    }
  }
);

const steps = document.getElementById('keyword-calls');           
new Sortable(
  steps, 
  {
    group: 
    {
      name: 'shared',
    },
    removeOnSpill: true,    
    animation: 150,
    handle: ".handle",
    onAdd: function(event) {
      event.item.innerHTML = keywordCall(event.item.id)
      event.item.style = "cursor: auto"
    },
  }
);
