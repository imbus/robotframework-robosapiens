import json
import os
from RoboSAPiens import RoboSAPiensClient
from django.http import HttpRequest
from django.shortcuts import render
from robot.errors import RemoteError
from django.conf import settings


de_json = os.path.join(settings.BASE_DIR, 'static', 'de.json')

with open(de_json, encoding='utf-8') as file:
    spec = json.load(file)

robosapiens = RoboSAPiensClient({})

def run_keyword(request: HttpRequest):
    keyword = request.POST['keyword']
    args = [
        param
        for name, param, in request.POST.items()
        if name not in ['keyword']
    ]

    try:
        kw_result = spec["keywords"][keyword]["result"]
        result = robosapiens._run_keyword(keyword, args, kw_result) or kw_result["Pass"].format(*args)
        error = None
    except RemoteError as err:
        result = None
        error = err.message

    return render(request, 'keyword_result.html', {"error": error, "result": result, "keyword": keyword})
