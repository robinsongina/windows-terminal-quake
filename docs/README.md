# Setup python virtual env
https://python.land/virtual-environments/virtualenv

Eg.
```ps1
py -m venv py3
```

# Activate virtual env

Eg.
```ps1
py3\Scripts\Activate.ps1
```

```sh
source py3/bin/activate
```

# Install dependencies

Under project folder:

```python
pip install -r requirements.txt
```

# Run dev server

```ps1
mkdocs serve
```

# Build docs

```ps1
mkdocs build
```

# Upgrade Pip packages

```ps1
pip install pip-upgrader
pip-upgrader
```
