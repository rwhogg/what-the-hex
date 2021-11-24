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
