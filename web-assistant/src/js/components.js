import api from '../api.json';

function keywordCall(name) {
    const keyword = api.keywords[name];
    var args = '';
    for (const arg of Object.keys(keyword.args)) {
      args += `
        <div>
          <input type="text" name="${arg}" placeholder="${arg}">
        </div>
      `;
    }

    return `
      <div class="row row-cols-auto">
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
  for (const keyword of Object.keys(api.keywords)) {
    keywords += `<div class="list-group-item" id="${keyword}">${api.keywords[keyword].name}</div>`;
  }

  return `<div class="list-group">${keywords}</div>`;
}

const keywords = document.getElementById("keywords");
keywords.innerHTML = keywordList()

const keywordCalls = document.getElementById("keyword-calls");
keywordCalls.innerHTML += keywordCall("AttachToRunningSAP");
keywordCalls.innerHTML += keywordCall("FillTextField");
