import api from '../api.json';
import {html} from 'code-tag';
import Sortable from 'sortablejs'


function keywordCall(name) {
    const keyword = api.keywords[name];
    var args = '';
    for (const arg of Object.keys(keyword.args)) {
      args += html`
        <div>
          <input type="text" name="${arg}" placeholder="${arg}">
        </div>
      `;
    }

    return html`
      <div id="${name}" class="row row-cols-auto list-group-item">
        <div >
          <button class="primary center" onclick="callKeyword('${name}')" style="font-size:1rem;">▶</button>
        </div>
        <div >
          <p>${keyword.name}</p>
        </div>
        ${args}
      </div>
    `;

    // <div>
    // <p style="font-size:2rem;">🖼</p>
    // </div>
  }

function keywordList() {
  var keywords = '';
  for (const keyword of Object.keys(api.keywords).slice(0,4)) {
    keywords += html`<div class="list-group-item" id="${keyword}">${api.keywords[keyword].name}</div>`;
  }

  return html`<div id="keyword-list" class="list-group">${keywords}</div>`;
}

const keywords = document.getElementById("keywords");
keywords.innerHTML = keywordList()

const keyword_list = document.getElementById("keyword-list");
new Sortable(keyword_list, {
    group: {
    name: 'shared',
    pull: 'clone', // To clone: set pull to 'clone'
    put: false
    },
animation: 150,
sort: false // To disable sorting: set sort to falseSAP starten
});  


var steps = document.getElementById('keyword-calls');           
new Sortable(steps, {
    group: {
    name: 'shared',
    },
removeOnSpill: true,    
animation: 150,
onAdd: function(event){
  steps.innerHTML += keywordCall(event.item.id)
  // Sortable.clone.innerHTML = keywordCall(event.item.id)
  Sortable._hideClone()
},
removeCloneOnHide: false
});   