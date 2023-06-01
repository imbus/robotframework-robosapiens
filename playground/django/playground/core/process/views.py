from django.shortcuts import render

def edit_process(request):
    return render(request, 'process.html', {})