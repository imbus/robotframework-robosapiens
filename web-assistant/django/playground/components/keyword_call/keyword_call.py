from django_components import component

@component.register("keyword_call")
class KeywordList(component.Component):
    template_name = "keyword_call/keyword_call.html"

    def get_context_data(self):
        return {}

    class Media:
        css = "keyword_call/keyword_call.css"
        js = "keyword_call/keyword_call.js"
