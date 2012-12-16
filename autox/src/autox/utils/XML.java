package autox.utils;

/**
 * Created with AutoX project.
 * User: jien.huang
 * Date: 12/15/12
 */
import org.jdom.Document;
import org.jdom.Element;
import org.jdom.JDOMException;
import org.jdom.input.SAXBuilder;
import org.jdom.output.Format;
import org.jdom.output.XMLOutputter;
import org.xml.sax.InputSource;

import java.io.*;

public class XML {
    private final SAXBuilder _builder = new SAXBuilder();

    protected Document _document = null;

    private File originalFile = null;

    /**
     * @throws IOException
     * @throws JDOMException
     * @throws IOException
     * @throws org.xml.sax.SAXException
     */
    public XML(File file) throws JDOMException, IOException {

        _document = _builder.build(file);
        originalFile = file;

    }

    public XML(InputStream is) throws JDOMException, IOException {

        _document = _builder.build(is);

    }

    public XML(InputSource is) throws JDOMException, IOException {

        _document = _builder.build(is);

    }

    public XML(String content) throws JDOMException, IOException {
        StringReader sr = new StringReader(content);
        InputSource is = new InputSource(sr);
        _document = _builder.build(is);

    }

    public static Element getRoot(Element e) {
        if (e.getParentElement() == null)
            return e;
        else
            return getRoot(e.getParentElement());
    }
    public void setRoot(Element e) {
        _document.setRootElement(e);
    }

    public Element getRoot() {
        return _document.getRootElement();
    }

    public String toString() {

        XMLOutputter outer = new XMLOutputter();
        Format format = Format.getPrettyFormat();
        format.setIndent("\t");
        outer.setFormat(format);

        return outer.outputString(_document);

    }

    public static String toString(Element e) {
        XMLOutputter outer = new XMLOutputter();
        // outer.setFormat(Format.getCompactFormat());
        Format format = Format.getPrettyFormat();
        format.setIndent("\t");
        outer.setFormat(format);
        return outer.outputString(e);
    }

    public String toCompactString() {

        XMLOutputter outer = new XMLOutputter();

        outer.setFormat(Format.getCompactFormat());
        return outer.outputString(_document);

    }

    public static String toCompactString(Element e) {
        XMLOutputter outer = new XMLOutputter();
        outer.setFormat(Format.getCompactFormat());
        // outer.setFormat(Format.getPrettyFormat());
        return outer.outputString(e);
    }

    public void toFile() throws IOException {
        if (originalFile == null)
            throw new IOException("This XML did not know the original file!");
        toFile(originalFile);
    }

    public void toFile(File file) throws IOException {
        FileOutputStream fos = new FileOutputStream(file);
        fos.write(this.toString().getBytes());
    }

    public void toFile(String fileName) throws IOException {
        File file = new File(fileName);
        this.toFile(file);
    }
    public static Element getTopParent(Element e) {
        Element parent = e.getParentElement();
        if (parent == null)
            return e;
        else
            return getTopParent(parent);

    }
}
