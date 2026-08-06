*** Settings ***
{%- for name, args in settings|items %}
{{name}}    {{args}}
{%- endfor %}

{% if keywords %}
*** Keywords ***
{%- for keyword in keywords %}
{{keyword.name}}
    {%- for step in keyword.steps %}
    {%- if step.comment %}
    # {{step.comment}}
    {%- endif %}
    {{step}}
    {%- endfor %}
{% endfor %}
{% endif %}
*** Test Cases ***
{%- for testCase in testCases %}
{{testCase.name}}
    {%- for step in testCase.steps %}
    {{step}}
    {%- endfor %}
{%- endfor %}
