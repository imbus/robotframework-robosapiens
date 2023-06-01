import json
import os
from django.http import HttpRequest
from django.shortcuts import render
from django.conf import settings

de_json = os.path.join(settings.BASE_DIR, 'static', 'de.json')

with open(de_json, encoding='utf-8') as file:
    spec = json.load(file)

def keyword_call(request: HttpRequest, id: str):
    keyword = spec["keywords"][id]

    return render(request, "keyword_call/keyword_call.html", {'keyword': keyword})
