import json
import os
from django_components import component
from django.conf import settings

de_json = os.path.join(settings.BASE_DIR, 'static', 'de.json')

with open(de_json, encoding='utf-8') as file:
    spec = json.load(file)


@component.register("keyword_list")
class KeywordList(component.Component):
    template_name = "keyword_list/keyword_list.html"

    def get_context_data(self):
        keywords = [
            {
                'id': id,
                'name': keyword['name']
            }
            for id, keyword in spec['keywords'].items()
        ]

        return {
            "keywords": keywords,
        }

    class Media:
        css = "keyword_list/keyword_list.css"
        js = "keyword_list/keyword_list.js"
