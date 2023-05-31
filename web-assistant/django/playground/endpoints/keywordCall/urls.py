from django.urls import path
from . import views

urlpatterns = [
    path('keywordCall/<str:id>', views.keyword_call, name='endpoints.keywordCall'),
]
