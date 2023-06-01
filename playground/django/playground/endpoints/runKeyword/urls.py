from django.urls import path
from . import views

urlpatterns = [
    path('runKeyword', views.run_keyword, name='endpoints.runKeyword'),
]
