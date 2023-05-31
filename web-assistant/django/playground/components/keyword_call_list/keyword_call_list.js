const steps = document.getElementById('keyword-call-list');           
new Sortable(
    steps, 
    {
        group: { name: 'shared' },
        removeOnSpill: true,
        animation: 150,
        handle: ".handle",
        onAdd: function(event) {
            fetch(`/keywordCall/${event.item.id}`)
            .then(response => response.text())
            .then(innerHTML => {
                event.item.innerHTML = innerHTML;
                event.item.style = "cursor: auto";
                event.item.id += `-${event.newIndex}`;
                htmx.process(event.item);
            });
        },
    }
);