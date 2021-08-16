# Notes

## HTML 5 Support

* When exporting, add the following to the HTML head include:

```html
<script>
try {
    screen.orientation.lock("landscape")
} catch(e) {
}
</script>
```

and set the canvas resize policy to "Project"

* Progressive web app support is waiting on Godot 3.4 to enter at least RC status.
  Not sure if I'll update to that when it comes out or just wait for stable.
