import api from '../api.json';
import {html} from 'code-tag';
import {Sortable, OnSpill} from 'sortablejs/modular/sortable.core.esm';

Sortable.mount(OnSpill);

function keywordCall(name) {
    const keyword = api.keywords[name];
    var args = '<div style="padding: 2px">';
    const keyword_args = Object.entries(keyword.args);
    for (const [arg_id, arg] of keyword_args) {
      args += html`
          <input type="text" name="${arg_id}" placeholder="${arg.name}" style="width: ${100/keyword_args.length - 1}%">
      `;
    }
    args += '</div>';
 
    const keywordCall_innerHTML = html`
      <div class="row g-1">
        <div class="handle col-1" style="cursor: grab; width: auto; font-size:13px;">📌</div>
        <div id="${name}" class="col">
          <button class="primary center" onclick="callKeyword('${name}')" style="font-size:1rem; dispay:inline; border: 0px; padding: 0px; margin: 0px; background-color: #fff;">▶</button>
          <span>${keyword.name}</span>
          <!-- <span style="font-size:1rem;">🖼</span> -->
          ${args}
        </div>
      <div>
    `;

    return keywordCall_innerHTML;
  }

function keywordList() {
  var keywords = '';
  for (const keyword of Object.keys(api.keywords)) {
    keywords += html`<div class="list-group-item" id="${keyword}" style="cursor: grab">${api.keywords[keyword].name}</div>`;
  }

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


var steps = document.getElementById('keyword-calls');           
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
