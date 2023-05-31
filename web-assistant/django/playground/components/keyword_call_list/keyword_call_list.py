from django_components import component

@component.register("keyword_call_list")
class KeywordList(component.Component):
    template_name = "keyword_call_list/keyword_call_list.html"

    def get_context_data(self):
        return {"keyword_calls": []}

    class Media:
        css = "keyword_call_list/keyword_call_list.css"
        js = "keyword_call_list/keyword_call_list.js"
