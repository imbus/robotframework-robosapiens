

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